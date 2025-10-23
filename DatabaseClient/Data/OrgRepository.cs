using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DatabaseClient.Models;

namespace DatabaseClient.Data;

public class OrgRepository
{
    // Implementation of OrgRepository
    private readonly String _connectionString;

    public OrgRepository()
    {
        _connectionString = ConfigService.GetConnection("OrgDB");
    }

    // 1. Get all employees
    public List<Employee> GetEmployees()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string is not initialized.");
        }
        try
        {
            var table = SqlServerConnection.ExecuteStoredProcedure(
                _connectionString,
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
                        HireDate = row.Field<DateTime>("HireDate"),
                    }),
            ];
        }
        catch (Exception ex)
        {
            // Log exception (not implemented here)
            throw new DataException("Error retrieving employees.", ex);
        }
    }

    // 2. Get employee by ID
    public Employee GetEmployeeById(int empId)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string is not initialized.");
        }
        try
        {
            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _connectionString,
                "GetEmployeeById",
                new Microsoft.Data.SqlClient.SqlParameter("@EmpID", empId)
            );
            if (reader == null || !reader.Read())
            {
                throw new KeyNotFoundException($"Employee with ID {empId} not found.");
            }
            return new Employee
            {
                EmpID = reader.GetInt32(reader.GetOrdinal("EmpID")),
                EmployeeNO = reader["EmployeeNO"] as string ?? string.Empty,
                FirstName = reader["FirstName"] as string ?? string.Empty,
                LastName = reader["LastName"] as string ?? string.Empty,
                Email = reader["Email"] as string ?? string.Empty,
                DivisionID = reader.GetInt32(reader.GetOrdinal("DivisionID")),
                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
            };
        }
        catch (Exception ex)
        {
            // Log exception (not implemented here)
            throw new DataException("Error retrieving employee.", ex);
        }
    }
}
