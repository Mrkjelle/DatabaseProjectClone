using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using DatabaseClient.Models.Org;

namespace DatabaseClient.Data;

public class OrgRepository : BaseRepository
{
    public OrgRepository()
        : base(ConfigService.GetConnection("OrgDB"))
    {
        Console.WriteLine($"[DEBUG] OrgDB connection: {_primaryConnectionString}");
    }

    // 1. Get all employees
    public List<Employee> GetEmployees()
    {
        EnsureConnection();
        try
        {
            var table = SqlServerConnection.ExecuteStoredProcedureTable(
                _primaryConnectionString,
                "GetEmployees"
            );

            return
            [
                .. table
                    .AsEnumerable()
                    .Select(row => new Employee
                    {
                        EmpID = row.Field<int>("EmpID"),
                        EmployeeNO = row.Field<string>("EmployeeNO") ?? string.Empty,
                        FirstName = row.Field<string>("FirstName") ?? string.Empty,
                        LastName = row.Field<string>("LastName") ?? string.Empty,
                        Email = row.Field<string>("Email") ?? string.Empty,
                        DivisionID = row.Field<int>("DivisionID"),
                        DivisionCode = row.Field<string>("DivisionCode") ?? string.Empty,
                        HireDate = row.Field<DateTime>("HireDate"),
                    }),
            ];
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error retrieving employees.", ex);
        }
    }
}
