using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DatabaseClient.Data;

public class ProjectRepository : BaseRepository
{
    public ProjectRepository() : base(ConfigService.GetConnection("ProjectDB")) { }
}