namespace DatabaseClient.Models;

public class Division
{
    public int DivisionID { get; set; }
    public string DivisionCode { get; set; } = string.Empty;
    public string DivisionName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public Division() { }

    public Division(int divisionID, string divisionCode, string divisionName, string location)
    {
        DivisionID = divisionID;
        DivisionCode = divisionCode;
        DivisionName = divisionName;
        Location = location;
    }
}
