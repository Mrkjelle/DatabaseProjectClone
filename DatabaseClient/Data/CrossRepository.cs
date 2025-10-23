using System;

namespace DatabaseClient.Data;

public class CrossRepository : BaseRepository
{
    private readonly string _orgConnection;
    private readonly string _projectConnection;
    public CrossRepository()
        : base(ConfigService.GetConnection("OrgDB"), ConfigService.GetConnection("ProjectDB"))
    {
        _orgConnection = _primaryConnectionString;
        _projectConnection = _secondaryConnectionString!;
    }
}