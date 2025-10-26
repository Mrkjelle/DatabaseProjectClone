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
                        Budget = row.Field<decimal>("Budget"),
                        StartDate = row.Field<DateTime>("StartDate"),
                        EndDate = row.Field<DateTime?>("EndDate"),
                    }),
            ];
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error retrieving projects from the database.", ex);
        }
    }

    public Project GetProjectById(int projectId)
    {
        EnsureConnection();
        try
        {
            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _primaryConnectionString,
                "GetProjectById",
                new SqlParameter("@ProjectID", projectId)
            );
            if (reader == null || !reader.Read())
            {
                throw new KeyNotFoundException($"Project with ID {projectId} not found.");
            }
            return new Project
            {
                ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                ProjectCode = reader["ProjectCode"] as string ?? string.Empty,
                ProjectName = reader["ProjectName"] as string ?? string.Empty,
                Budget = reader.GetDecimal(reader.GetOrdinal("Budget")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("EndDate")),
            };
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error retrieving project with ID {projectId} from the database.",
                ex
            );
        }
    }

    public int AddProject(Project project)
    {
        EnsureConnection();
        try
        {
            var result = SqlServerConnection.ExecuteStoredProcedureScalar(
                _primaryConnectionString,
                "AddProject",
                new SqlParameter("@ProjectCode", project.ProjectCode),
                new SqlParameter("@ProjectName", project.ProjectName),
                new SqlParameter("@Budget", project.Budget),
                new SqlParameter("@StartDate", project.StartDate),
                new SqlParameter(
                    "@EndDate",
                    project.EndDate.HasValue ? (object)project.EndDate.Value : DBNull.Value
                )
            );
            int newProjectId = Convert.ToInt32(result);

            return newProjectId;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error adding new project to the database.", ex);
        }
    }

    public void UpdateProject(Project project)
    {
        EnsureConnection();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _primaryConnectionString,
                "UpdateProject",
                new SqlParameter("@ProjectID", project.ProjectID),
                new SqlParameter("@ProjectCode", project.ProjectCode),
                new SqlParameter("@ProjectName", project.ProjectName),
                new SqlParameter("@Budget", project.Budget),
                new SqlParameter("@StartDate", project.StartDate),
                new SqlParameter(
                    "@EndDate",
                    project.EndDate.HasValue ? (object)project.EndDate.Value : DBNull.Value
                )
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error updating project with ID {project.ProjectID} in the database.",
                ex
            );
        }
    }

    public void DeleteProject(int projectId)
    {
        EnsureConnection();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _primaryConnectionString,
                "DeleteProject",
                new SqlParameter("@ProjectID", projectId)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error deleting project with ID {projectId} from the database.",
                ex
            );
        }
    }

    public void DeleteProjectCascade(int projectId)
    {
        EnsureConnection();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _primaryConnectionString,
                "DeleteProjectCascade",
                new SqlParameter("@ProjectID", projectId)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error deleting project with ID {projectId} from the database.",
                ex
            );
        }
    }
}
