using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;
using DatabaseClient.Models.Proj;

namespace DatabaseClient.Data;

public partial class CrossRepository : BaseRepository
{
    private readonly string _orgConnection;
    private readonly string _projectConnection;
    private readonly OrgRepository _orgRepo;
    private readonly ProjectRepository _projectRepo;

    public CrossRepository()
        : base(ConfigService.GetConnection("OrgDB"), ConfigService.GetConnection("ProjectDB"))
    {
        _orgConnection = _primaryConnectionString;
        _projectConnection = _secondaryConnectionString!;
        _orgRepo = new OrgRepository();
        _projectRepo = new ProjectRepository();
    }

    public List<Division> GetDivisionsForProject(int projectId)
    {
        EnsureBothConnections();
        try
        {
            var divisions = new List<Division>();

            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _projectConnection,
                "GetDivisionsByProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", projectId)
            );

            if (reader == null)
            {
                throw new DataException("No data returned from the database.");
            }

            while (reader.Read())
            {
                int divId = reader.GetInt32(reader.GetOrdinal("DivisionFK"));
                var divDetails = _orgRepo.GetDivisionById(divId);
                if (divDetails != null)
                {
                    divisions.Add(divDetails);
                }
            }
            return divisions;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error retrieving divisions for the specified project: {projectId}",
                ex
            );
        }
    }

    public List<Project> GetProjectsByDivision(int divisionId)
    {
        EnsureBothConnections();
        try
        {
            var projects = new List<Project>();

            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _projectConnection,
                "GetProjectsByDivision",
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionFK", divisionId)
            );
            if (reader == null)
            {
                throw new DataException("No data returned from the database.");
            }
            while (reader.Read())
            {
                int projId = reader.GetInt32(reader.GetOrdinal("ProjectFK"));
                var projDetails = _projectRepo.GetProjectById(projId);
                if (projDetails != null)
                {
                    projects.Add(projDetails);
                }
            }
            return projects;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error retrieving projects for the specified division: {divisionId}",
                ex
            );
        }
    }

    public List<Project> GetProjectsByEmployee(int employeeId)
    {
        EnsureBothConnections();
        try
        {
            var projects = new List<Project>();

            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _projectConnection,
                "GetProjectsByEmployee",
                new Microsoft.Data.SqlClient.SqlParameter("@EmployeeFK", employeeId)
            );
            if (reader == null)
            {
                throw new DataException("No data returned from the database.");
            }
            while (reader.Read())
            {
                int projId = reader.GetInt32(reader.GetOrdinal("ProjectFK"));
                var projDetails = _projectRepo.GetProjectById(projId);
                if (projDetails != null)
                {
                    projects.Add(projDetails);
                }
            }
            return projects;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error retrieving projects for the specified employee: {employeeId}",
                ex
            );
        }
    }

    public void AddDivisionToProject(int projectId, int divisionId)
    {
        EnsureBothConnections();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _projectConnection,
                "AddDivisionToProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", projectId),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionFK", divisionId)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error adding division {divisionId} to project {projectId}.",
                ex
            );
        }
    }

    public void RemoveDivisionFromProject(int ProjectFK, int DivisionFK)
    {
        EnsureBothConnections();
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.DefaultTimeout,
            },
            TransactionScopeAsyncFlowOption.Enabled
        );
        try
        {
            var employees = GetEmployeesByProject(ProjectFK);
            foreach (var emp in employees)
            {
                if (emp.DivisionID == DivisionFK)
                {
                    RemoveEmployeeFromProject(ProjectFK, emp.EmpID);
                }
            }
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _projectConnection,
                "RemoveDivisionFromProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", ProjectFK),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionFK", DivisionFK)
            );
            scope.Complete();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error removing division {DivisionFK} from project {ProjectFK}.",
                ex
            );
        }
    }

    public EmployeeProject GetEmployeeProject(int ProjectFK, int EmployeeFK)
    {
        EnsureBothConnections();
        try
        {
            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _projectConnection,
                "GetEmployeeProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", ProjectFK),
                new Microsoft.Data.SqlClient.SqlParameter("@EmployeeFK", EmployeeFK)
            );

            if (reader == null || !reader.Read())
            {
                throw new DataException("No data returned from the database.");
            }

            return new EmployeeProject
            {
                EmpProjID = reader.GetInt32(reader.GetOrdinal("EmpProjID")),
                EmpFK = reader.GetInt32(reader.GetOrdinal("EmpFK")),
                ProjectFK = reader.GetInt32(reader.GetOrdinal("ProjectFK")),
                Role = reader.GetString(reader.GetOrdinal("Role")),
                HoursWorked = reader.GetDecimal(reader.GetOrdinal("HoursWorked")),
            };
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error retrieving EmployeeProject for EmployeeFK {EmployeeFK} and ProjectFK {ProjectFK}.",
                ex
            );
        }
    }

    public void UpdateEmployeeProject(
        int ProjectFK,
        int EmployeeFK,
        string Role,
        decimal HoursWorked
    )
    {
        EnsureBothConnections();
        try
        {
            int empProjID = GetEmployeeProject(ProjectFK, EmployeeFK).EmpProjID;
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _projectConnection,
                "UpdateEmployeeProject",
                new Microsoft.Data.SqlClient.SqlParameter("@EmpProjID", empProjID),
                new Microsoft.Data.SqlClient.SqlParameter("@Role", Role),
                new Microsoft.Data.SqlClient.SqlParameter("@HoursWorked", HoursWorked)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error updating EmployeeProject for EmployeeFK {EmployeeFK} and ProjectFK {ProjectFK}.",
                ex
            );
        }
    }

    public void DeleteEmployeeWithCleanup(int employeeId)
    {
        EnsureBothConnections();
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.DefaultTimeout,
            },
            TransactionScopeAsyncFlowOption.Enabled
        );

        try
        {
            var projects = GetProjectsByEmployee(employeeId);
            if (projects != null)
            {
                foreach (var project in projects)
                {
                    RemoveEmployeeFromProject(project.ProjectID, employeeId);
                }
            }

            _orgRepo.DeleteEmployee(employeeId);

            scope.Complete();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error deleting employee with cleanup.", ex);
        }
    }
}
