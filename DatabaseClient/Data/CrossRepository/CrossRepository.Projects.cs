using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseClient.Models.Proj;

namespace DatabaseClient.Data;

public partial class CrossRepository
{
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
}
