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

    public void WarmUp()
    {
        EnsureConnection();
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

    // 2. Get employee by ID
    public Employee GetEmployeeById(int empId)
    {
        EnsureConnection();
        try
        {
            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _primaryConnectionString,
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
            LogError(ex);
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
                _primaryConnectionString,
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
            LogError(ex);
            throw new DataException("Error adding new employee.", ex);
        }
    }

    // 4. Update existing Employee in database
    public void UpdateEmployee(Employee employee)
    {
        EnsureConnection();
        SqlServerConnection.ExecuteStoredProcedureSimple(
            _primaryConnectionString,
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

    // 5. Delete Employee from database
    public void DeleteEmployee(int empId)
    {
        EnsureConnection();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _primaryConnectionString,
                "DeleteEmployee",
                new Microsoft.Data.SqlClient.SqlParameter("@EmpID", empId)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error deleting employee.", ex);
        }
    }

    // 6. Get Divisions
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
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error retrieving divisions.", ex);
        }
    }

    // 7. Get Division by ID
    public Division GetDivisionById(int divisionId)
    {
        EnsureConnection();
        try
        {
            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _primaryConnectionString,
                "GetDivisionById",
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionID", divisionId)
            );
            if (reader == null || !reader.Read())
            {
                throw new KeyNotFoundException($"Division with ID {divisionId} not found.");
            }
            return new Division
            {
                DivisionID = reader.GetInt32(reader.GetOrdinal("DivisionID")),
                DivisionCode = reader["DivisionCode"] as string ?? string.Empty,
                DivisionName = reader["DivisionName"] as string ?? string.Empty,
                Location = reader["Location"] as string ?? string.Empty,
            };
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error retrieving division.", ex);
        }
    }

    // 8. Add Division
    public int AddDivision(Division division)
    {
        EnsureConnection();
        try
        {
            var result = SqlServerConnection.ExecuteStoredProcedureScalar(
                _primaryConnectionString,
                "AddDivision",
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionCode", division.DivisionCode),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionName", division.DivisionName),
                new Microsoft.Data.SqlClient.SqlParameter("@Location", division.Location)
            );
            int newId = Convert.ToInt32(result);
            division.DivisionID = newId;

            return newId;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error adding new division.", ex);
        }
    }

    //9. Update Division
    public void UpdateDivision(Division division)
    {
        EnsureConnection();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _primaryConnectionString,
                "UpdateDivision",
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionID", division.DivisionID),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionCode", division.DivisionCode),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionName", division.DivisionName),
                new Microsoft.Data.SqlClient.SqlParameter("@Location", division.Location)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error updating division.", ex);
        }
    }

    //10. Delete Division
    public void DeleteDivision(int divisionId)
    {
        EnsureConnection();
        try
        {
            var projects = new CrossRepository().GetProjectsByDivision(divisionId);
            if (projects != null && projects.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Cannot delete division with ID {divisionId} because it has associated projects."
                );
            }

            SqlServerConnection.ExecuteStoredProcedureSimple(
                _primaryConnectionString,
                "DeleteDivision",
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionID", divisionId)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException("Error deleting division.", ex);
        }
    }
}
