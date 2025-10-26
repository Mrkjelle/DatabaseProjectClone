using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseClient.Models.Proj;
using Microsoft.Data.SqlClient;

namespace DatabaseClient.Data;

public class ProjectRepository : BaseRepository
{
    public ProjectRepository()
        : base(ConfigService.GetConnection("OrgDB")) { }

    public List<Project> GetProjects()
    {
        EnsureConnection();
        try
        {
            var table = SqlServerConnection.ExecuteStoredProcedureTable(
                _primaryConnectionString,
                "GetProjects"
            );

            return
            [
                .. table
                    .AsEnumerable()
                    .Select(row => new Project
                    {
                        ProjectID = row.Field<int>("ProjectID"),
                        ProjectCode = row.Field<string>("ProjectCode") ?? string.Empty,
                        ProjectName = row.Field<string>("ProjectName") ?? string.Empty,
                        StartDate = row.Field<DateTime>("StartDate"),
                        EndDate = row.Field<DateTime?>("EndDate"),
                        Budget = row.Field<decimal>("Budget"),
                    }),
            ];
        }
        catch (SqlException sqlEx)
        {
            LogError(sqlEx);
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public List<EmployeeProject> GetEmployeeProjects()
    {
        EnsureConnection();
        try
        {
            var table = SqlServerConnection.ExecuteStoredProcedureTable(
                _primaryConnectionString,
                "GetEmployeeProjects"
            );

            return
            [
                .. table
                    .AsEnumerable()
                    .Select(row => new EmployeeProject
                    {
                        EmpProjID = row.Field<int>("EmpProjID"),
                        EmpFK = row.Field<int>("EmpFK"),
                        ProjectFK = row.Field<int>("ProjectFK"),
                        Role = row.Field<string>("Role") ?? string.Empty,
                        HoursWorked = row.Field<decimal>("HoursWorked"),
                    }),
            ];
        }
        catch (SqlException sqlEx)
        {
            LogError(sqlEx);
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public List<DivisionProject> GetDivisionProjects()
    {
        EnsureConnection();
        try
        {
            var table = SqlServerConnection.ExecuteStoredProcedureTable(
                _primaryConnectionString,
                "GetDivisionProjects"
            );

            return
            [
                .. table
                    .AsEnumerable()
                    .Select(row => new DivisionProject
                    {
                        DivisionProjectID = row.Field<int>("DivisionProjectID"),
                        DivisionFK = row.Field<int>("DivisionFK"),
                        ProjectFK = row.Field<int>("ProjectFK"),
                        DivisionCode = row.Field<string>("DivisionCode") ?? string.Empty,
                        DivisionName = row.Field<string>("DivisionName") ?? string.Empty,
                        ProjectCode = row.Field<string>("ProjectCode") ?? string.Empty,
                        ProjectName = row.Field<string>("ProjectName") ?? string.Empty,
                    }),
            ];
        }
        catch (SqlException sqlEx)
        {
            LogError(sqlEx);
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public List<Project> GetAssignableProjectsForEmployee(int empId)
    {
        var table = SqlServerConnection.ExecuteStoredProcedureTable(
            _primaryConnectionString, // <— now correct
            "GetAssignableProjectsForEmployee",
            new SqlParameter("@EmpID", empId)
        );

        var projects = new List<Project>();
        foreach (DataRow row in table.Rows)
        {
            projects.Add(
                new Project
                {
                    ProjectID = Convert.ToInt32(row["ProjectID"]),
                    ProjectCode = row["ProjectCode"].ToString()!,
                    ProjectName = row["ProjectName"].ToString()!,
                }
            );
        }
        return projects;
    }
    public void AddEmployeeToProject(int empId, int projectId)
    {
        EnsureConnection();
        try
        {
            var parameters = new[]
            {
                new SqlParameter("@EmpID", empId),
                new SqlParameter("@ProjectID", projectId),
            };
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _primaryConnectionString,
                "AddEmployeeToProject",
                parameters
            );
        }
        catch (SqlException sqlEx)
        {
            LogError(sqlEx);
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }
}
