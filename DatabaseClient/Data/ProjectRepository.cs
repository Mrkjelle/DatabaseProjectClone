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
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error retrieving projects.", ex);
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
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error retrieving employee projects.", ex);
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
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error retrieving division projects.", ex);
        }
    }
}
