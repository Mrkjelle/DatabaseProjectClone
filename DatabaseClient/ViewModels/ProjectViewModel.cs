using System;
using System.Collections.ObjectModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Proj;

namespace DatabaseClient.ViewModels;

public class ProjectViewModel
{
    public ObservableCollection<Project> Projects { get; } = new();

    public ProjectViewModel()
    {
        LoadProjects();
    }

    private void LoadProjects()
    {
        try
        {
            var repo = new ProjectRepository();
            var projects = repo.GetProjects();

            Projects.Clear();
            foreach (var proj in projects)
            {
                Console.WriteLine($"Loading project: {proj.ProjectName}");
                Projects.Add(proj);
            }
            Console.WriteLine($"Total projects loaded: {Projects.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading projects: {ex.Message}");
        }
    }
}
