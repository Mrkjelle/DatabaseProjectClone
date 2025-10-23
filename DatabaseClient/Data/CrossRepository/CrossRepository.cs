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
}
