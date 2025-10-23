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

    private void EnsureConnection()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string is not initialized.");
        }
    }

    // 1. Get all employees
    public List<Employee> GetEmployees()
    {
        EnsureConnection();
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
        EnsureConnection();
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

    // 3. Adds new Employee to database
    public int AddEmployee(Employee employee)
    {
        EnsureConnection();
        try
        {
            var result = SqlServerConnection.ExecuteStoredProcedureScalar(
                _connectionString,
                "AddEmployee",
                new Microsoft.Data.SqlClient.SqlParameter("@EmployeeNO", employee.EmployeeNO),
                new Microsoft.Data.SqlClient.SqlParameter("@FirstName", employee.FirstName),
                new Microsoft.Data.SqlClient.SqlParameter("@LastName", employee.LastName),
                new Microsoft.Data.SqlClient.SqlParameter("@Email", employee.Email),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionID", employee.DivisionID),
                new Microsoft.Data.SqlClient.SqlParameter("@HireDate", employee.HireDate)
            );
            int newId = Convert.ToInt32(result);
            employee.EmpID = newId;

            return newId;
        }
        catch (Exception ex)
        {
            // Log exception (not implemented here)
            throw new DataException("Error adding new employee.", ex);
        }
    }

    // 4. Update existing Employee in database
    public void UpdateEmployee(Employee employee)
    {
        EnsureConnection();
        try
        {
            SqlServerConnection.ExecuteStoredProcedure(
                _connectionString,
                "UpdateEmployee",
                new Microsoft.Data.SqlClient.SqlParameter("@EmpID", employee.EmpID),
                new Microsoft.Data.SqlClient.SqlParameter("@EmployeeNO", employee.EmployeeNO),
                new Microsoft.Data.SqlClient.SqlParameter("@FirstName", employee.FirstName),
                new Microsoft.Data.SqlClient.SqlParameter("@LastName", employee.LastName),
                new Microsoft.Data.SqlClient.SqlParameter("@Email", employee.Email),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionID", employee.DivisionID),
                new Microsoft.Data.SqlClient.SqlParameter("@HireDate", employee.HireDate)
            );
        }
        catch (Exception ex)
        {
            // Log exception (not implemented here)
            throw new DataException("Error updating employee.", ex);
        }
    }
}
