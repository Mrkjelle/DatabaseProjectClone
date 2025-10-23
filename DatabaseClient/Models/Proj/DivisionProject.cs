namespace DatabaseClient.Models.Proj;

public class DivisionProject
{
    public int DivisionProjectID { get; set; }
    public int DivisionFK { get; set; }
    public int ProjectFK { get; set; }

    public DivisionProject() { }

    public DivisionProject(int divisionProjectID, int divisionFK, int projectFK)
    {
        DivisionProjectID = divisionProjectID;
        DivisionFK = divisionFK;
        ProjectFK = projectFK;
    }
}
