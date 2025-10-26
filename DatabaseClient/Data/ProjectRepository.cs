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
}
