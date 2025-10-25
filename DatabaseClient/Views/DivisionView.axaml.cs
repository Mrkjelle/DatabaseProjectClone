using Avalonia.Controls;
using System.Linq;
using Avalonia.Interactivity;
using DatabaseClient.ViewModels;
using DatabaseClient.Models.Org;
using Avalonia;

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
}
