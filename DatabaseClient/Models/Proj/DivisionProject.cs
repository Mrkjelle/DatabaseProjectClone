namespace DatabaseClient.Models.Proj;

public class DivisionProject
{
    public int DivisionProjectID { get; set; }
    public int DivisionFK { get; set; }
    public int ProjectFK { get; set; }
    public string DivisionCode { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;

    public DivisionProject() { }

    public DivisionProject(
        int divisionProjectID,
        int divisionFK,
        int projectFK,
        string divisionCode,
        string divisionName,
        string projectCode,
        string projectName
    )
    {
        DivisionProjectID = divisionProjectID;
        DivisionFK = divisionFK;
        ProjectFK = projectFK;
        DivisionCode = divisionCode;
        DivisionName = divisionName;
        ProjectCode = projectCode;
        ProjectName = projectName;
    }
}
