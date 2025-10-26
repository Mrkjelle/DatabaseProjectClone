using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DatabaseClient.Models.Proj;
using DatabaseClient.Data;
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
        if (e.PropertyName == "EmpFK")
        {
            e.Cancel = true;
        }
    }

    private void OnSwitchToDivisionAssignments(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AssignmentViewModel vm)
        {
            vm.ShowDivisionAssignments = !vm.ShowDivisionAssignments;
        }
        if (vm.ShowDivisionAssignments && vm.DivisionAssignments.Count == 0)
        {
            var repo = new ProjectRepository();
            var divAssignments = repo.GetDivisionProjects();
            foreach (var da in divAssignments)
            {
                vm.DivisionAssignments.Add(da);
            }
        }
    }
}
