namespace DatabaseClient.Models.Proj;

public class EmployeeProject
{
    public int EmpProjID { get; set; }
    public int EmpFK { get; set; }
    public int ProjectFK { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal HoursWorked { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DivisionCode { get; set; } = string.Empty;
    public int DivisionID { get; set; }

    public EmployeeProject() { }

    public EmployeeProject(
        int empProjID,
        int empFK,
        int projectFK,
        string role,
        decimal hoursWorked
    )
    {
        EmpProjID = empProjID;
        EmpFK = empFK;
        ProjectFK = projectFK;
        Role = role;
        HoursWorked = hoursWorked;
    }
}
