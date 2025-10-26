using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;
using DatabaseClient.Models.Proj;

namespace DatabaseClient.ViewModels;

public class AssignmentViewModel : INotifyPropertyChanged
{
    public ObservableCollection<EmployeeProject> EmployeeAssignments { get; } = new();
    public ObservableCollection<DivisionProject> DivisionAssignments { get; } = new();
    private bool _showDivisionAssignments;
    public bool ShowDivisionAssignments
    {
        get => _showDivisionAssignments;
        set
        {
            if (_showDivisionAssignments != value)
            {
                _showDivisionAssignments = value;
                OnPropertyChanged();
            }
        }
    }

    public AssignmentViewModel()
    {
        LoadEmployeeAssignments();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
