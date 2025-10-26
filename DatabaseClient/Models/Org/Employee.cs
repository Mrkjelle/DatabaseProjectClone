using System;
using System.ComponentModel;

namespace DatabaseClient.Models.Org;

public class Employee
{
    public int EmpID { get; set; }
    public string EmployeeNO { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DivisionCode { get; set; } = string.Empty;
    public int DivisionID { get; set; }
    public DateTime HireDate { get; set; } = DateTime.MinValue;

    public Employee() { }

    public Employee(
        int empID,
        string employeeNO,
        string firstName,
        string lastName,
        string email,
        int divisionID,
        string divisionCode,
        DateTime hireDate
    )
    {
        EmpID = empID;
        EmployeeNO = employeeNO;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        DivisionID = divisionID;
        DivisionCode = divisionCode;
        HireDate = hireDate;
    }
}
