using System;
using System.Collections.ObjectModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Proj;

namespace DatabaseClient.ViewModels;

public class AssignmentViewModel
{
    public ObservableCollection<EmployeeProject> EmployeeAssignments { get; } = new();

    public AssignmentViewModel()
    {
        LoadEmployeeAssignments();
    }

    private void LoadEmployeeAssignments()
    {
        try
        {
            var repo = new ProjectRepository();
            var employeeProjects = repo.GetEmployeeProjects();

            EmployeeAssignments.Clear();
            foreach (var ep in employeeProjects)
            {
                Console.WriteLine(
                    $"Loading employee project: {ep.ProjectFK} for EmployeeID: {ep.EmpFK}"
                );
                EmployeeAssignments.Add(ep);
            }
            Console.WriteLine($"Total employee projects loaded: {EmployeeAssignments.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading employee projects: {ex.Message}");
        }
    }
}
