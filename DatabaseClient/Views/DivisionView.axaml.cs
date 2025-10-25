using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DatabaseClient.Models.Org;
using DatabaseClient.ViewModels;

namespace DatabaseClient.Views;

public partial class DivisionView : UserControl
{
    public DivisionView()
    {
        InitializeComponent();
        DataContext = new ViewModels.DivisionViewModel();
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ViewModels.DivisionViewModel vm)
        {
            var sorted = vm.Divisions.OrderBy(div => div.DivisionCode).ToList();
            vm.Divisions.Clear();
            foreach (var div in sorted)
            {
                vm.Divisions.Add(div);
            }
        }
    }

    private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName == "EmployeeID" || e.PropertyName == "DivisionID")
        {
            e.Cancel = true;
        }
    }
}
