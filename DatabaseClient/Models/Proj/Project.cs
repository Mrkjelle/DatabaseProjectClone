using System;

namespace DatabaseClient.Models.Proj;

public class Project
{
    public int ProjectID { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; } = DateTime.MinValue;
    public DateTime? EndDate { get; set; } = null;

    public Project() { }

    public Project(
        int projectID,
        string projectCode,
        string projectName,
        decimal budget,
        DateTime startDate,
        DateTime? endDate
    )
    {
        ProjectID = projectID;
        ProjectCode = projectCode;
        ProjectName = projectName;
        Budget = budget;
        StartDate = startDate;
        EndDate = endDate;
    }
}