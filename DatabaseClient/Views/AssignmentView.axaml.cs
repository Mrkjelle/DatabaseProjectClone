using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DatabaseClient.Data;
using DatabaseClient.Models.Proj;
using DatabaseClient.ViewModels;

namespace DatabaseClient.Views;

public partial class AssignmentView : UserControl
{
    public AssignmentView()
    {
        InitializeComponent();
        DataContext = new ViewModels.AssignmentViewModel();
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ViewModels.AssignmentViewModel vm)
        {
            var sorted = vm.EmployeeAssignments.OrderBy(ep => ep.EmpFK).ToList();
            vm.EmployeeAssignments.Clear();
            foreach (var ep in sorted)
            {
                vm.EmployeeAssignments.Add(ep);
            }
        }
    }

    private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (
            e.PropertyName == "EmpFK"
            || e.PropertyName == "ProjectFK"
            || e.PropertyName == "DivisionFK"
            || e.PropertyName == "DivisionID"
            || e.PropertyName == "DivisionProjectID"
            || e.PropertyName == "EmpProjID"
        )
        {
            e.Cancel = true;
        }
    }

    private void OnSwitchToDivisionAssignments(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as AssignmentViewModel;
        if (vm == null)
            return;
        vm.ShowDivisionAssignments = !vm.ShowDivisionAssignments;
        if (vm.ShowDivisionAssignments && vm.DivisionAssignments.Count == 0)
        {
            var repo = new ProjectRepository();
            var divisionProjects = repo.GetDivisionProjects();
            foreach (var dp in divisionProjects)
            {
                Console.WriteLine(
                    $"Loading division project: {dp.ProjectFK} for DivisionID: {dp.DivisionFK}"
                );
                vm.DivisionAssignments.Add(dp);
            }
        }
    }
}
