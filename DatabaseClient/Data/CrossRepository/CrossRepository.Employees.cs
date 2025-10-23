using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using DatabaseClient.Models.Org;
using Microsoft.Data.SqlClient;

namespace DatabaseClient.Data;

public partial class CrossRepository : BaseRepository
{
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

    public void RemoveEmployeeFromProject(int ProjectFK, int EmployeeFK)
    {
        EnsureBothConnections();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _projectConnection,
                "RemoveEmployeeFromProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", ProjectFK),
                new Microsoft.Data.SqlClient.SqlParameter("@EmployeeFK", EmployeeFK)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error removing employee {EmployeeFK} from project {ProjectFK}.",
                ex
            );
        }
    }

    public void UpdateEmployeeWithDivisionCheck(Employee employee)
    {
        EnsureBothConnections();
        var existing = _orgRepo.GetEmployeeById(employee.EmpID);
        if (existing == null)
        {
            throw new KeyNotFoundException(
                $"Employee with ID {employee.EmpID} not found for update."
            );
        }

        bool divisionChanged = existing.DivisionID != employee.DivisionID;

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
            _orgRepo.UpdateEmployee(employee);

            if (divisionChanged)
            {
                HandleEmployeeDivisionChange(
                    employee.EmpID,
                    existing.DivisionID,
                    employee.DivisionID
                );
            }

            scope.Complete();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error updating employee with division check.", ex);
        }
    }

    public void HandleEmployeeDivisionChange(int employeeId, int oldDivisionId, int newDivisionId)
    {
        EnsureBothConnections();
        try
        {
            var currentProjects = GetProjectsByEmployee(employeeId);
            if (currentProjects == null || currentProjects.Count == 0)
            {
                return;
            }

            foreach (var project in currentProjects)
            {
                var divisionsInProject = GetDivisionsForProject(project.ProjectID);
                bool newDivisionAssigned = divisionsInProject.Exists(d =>
                    d.DivisionID == newDivisionId
                );

                if (!newDivisionAssigned)
                {
                    RemoveEmployeeFromProject(project.ProjectID, employeeId);
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error handling employee division change for employee {employeeId}.",
                ex
            );
        }
    }
}
