

using System;

namespace DatabaseClient.Data
{
    public class OrgRepository
    {
        // Implementation of OrgRepository
        private readonly String _connectionString;

        public OrgRepository()
        {
            _connectionString = ConfigService.GetConnection("OrgDB");
        }
    }
}