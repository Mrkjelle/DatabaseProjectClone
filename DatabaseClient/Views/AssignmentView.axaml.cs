using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
            var sorted = vm.EmployeeProjects.OrderBy(ep => ep.ProjectName).ToList();
            vm.EmployeeProjects.Clear();
            foreach (var ep in sorted)
            {
                vm.EmployeeProjects.Add(ep);
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
}
