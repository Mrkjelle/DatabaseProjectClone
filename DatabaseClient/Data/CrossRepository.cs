using System;
using System.Collections.Generic;
using System.Data;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;
using DatabaseClient.Models.Proj;

namespace DatabaseClient.Data;

public class CrossRepository : BaseRepository
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

    public List<Employee> GetEmployeesByProject(int projectId)
    {
        EnsureBothConnections();
        try
        {
            var employees = new List<Employee>();

            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _projectConnection,
                "GetEmployeesByProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", projectId)
            );

            if (reader == null)
            {
                throw new DataException("No data returned from the database.");
            }

            while (reader.Read())
            {
                int empId = reader.GetInt32(reader.GetOrdinal("EmpFK"));

                var empDetails = _orgRepo.GetEmployeeById(empId);
                if (empDetails != null)
                {
                    employees.Add(empDetails);
                }
            }
            return employees;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error retrieving employees for the specified project: {projectId}",
                ex
            );
        }
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

    public void AddEmployeeToProject(
        int projectId,
        int employeeId,
        string role,
        decimal hoursWorked
    )
    {
        EnsureBothConnections();
        try
        {
            var employee =
                _orgRepo.GetEmployeeById(employeeId)
                ?? throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            var projectDivisions = GetDivisionsForProject(projectId);
            if (projectDivisions == null || projectDivisions.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Project with ID {projectId} has no associated divisions."
                );
            }

            bool isDivisionAssigned = projectDivisions.Exists(d =>
                d.DivisionID == employee.DivisionID
            );

            if (!isDivisionAssigned)
            {
                throw new InvalidOperationException(
                    $"Employee's division (ID: {employee.DivisionID}) is not assigned to project {projectId}."
                        + "\n"
                        + "Cannot add employee to project."
                );
            }

            SqlServerConnection.ExecuteStoredProcedureSimple(
                _projectConnection,
                "AddEmployeeToProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", projectId),
                new Microsoft.Data.SqlClient.SqlParameter("@EmployeeFK", employeeId),
                new Microsoft.Data.SqlClient.SqlParameter("@Role", role),
                new Microsoft.Data.SqlClient.SqlParameter("@HoursWorked", hoursWorked)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error adding employee {employeeId} to project {projectId}.",
                ex
            );
        }
    }
}
