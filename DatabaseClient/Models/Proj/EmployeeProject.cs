namespace DatabaseClient.Models.Proj;

public class EmployeeProject
{
    public int EmpProjID { get; set; }
    public int EmpFK { get; set; }
    public int ProjectFK { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal HoursWorked { get; set; }

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
