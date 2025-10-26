using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using DatabaseClient.Models.Org;
using Microsoft.Data.SqlClient;

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

    // 2. Get Divisions
    public List<Division> GetDivisions()
    {
        EnsureConnection();
        try
        {
            var table = SqlServerConnection.ExecuteStoredProcedureTable(
                _primaryConnectionString,
                "GetDivisions"
            );

            return
            [
                .. table
                    .AsEnumerable()
                    .Select(row => new Division
                    {
                        DivisionID = row.Field<int>("DivisionID"),
                        DivisionCode = row.Field<string>("DivisionCode") ?? string.Empty,
                        DivisionName = row.Field<string>("DivisionName") ?? string.Empty,
                        Location = row.Field<string>("Location") ?? string.Empty,
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

    public int AddEmployee(Employee emp)
    {
        EnsureConnection();
        try
        {
            var parameters = new[]
            {
                new SqlParameter("@EmployeeNO", emp.EmployeeNO),
                new SqlParameter("@FirstName", emp.FirstName),
                new SqlParameter("@LastName", emp.LastName),
                new SqlParameter("@Email", emp.Email),
                new SqlParameter("@DivisionID", emp.DivisionID),
                new SqlParameter("@HireDate", emp.HireDate),
            };
            var result = SqlServerConnection.ExecuteStoredProcedureScalar(
                _primaryConnectionString,
                "AddEmployee",
                parameters
            );
            return Convert.ToInt32(result);
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
