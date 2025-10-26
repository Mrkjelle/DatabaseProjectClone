using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DatabaseClient.Models.Org;
using DatabaseClient.ViewModels;

namespace DatabaseClient.Views
{
    public partial class EmployeeView : UserControl
    {
        public EmployeeView()
        {
            InitializeComponent();
            DataContext = new EmployeeViewModel();

            this.AttachedToVisualTree += OnAttachedToVisualTree;
        }

        private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (DataContext is EmployeeViewModel vm)
            {
                var sorted = vm.Employees.OrderBy(emp => emp.EmployeeNO).ToList();
                vm.Employees.Clear();
                foreach (var emp in sorted)
                {
                    vm.Employees.Add(emp);
                }
            }
        }

        private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "EmpID" || e.PropertyName == "DivisionID")
            {
                e.Cancel = true;
            }
            else if (e.PropertyName == "HireDate")
            {
                var dateColumn = new DataGridTextColumn
                {
                    Header = "Hire Date",
                    Binding = new Avalonia.Data.Binding("HireDate")
                    {
                        Converter = new Utilities.DateOnlyConverter(),
                    },
                };
                e.Column = dateColumn;
            }
        }
        private void OnAddEmployeeClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is EmployeeViewModel vm)
            {
                vm.NewEmployee = new Employee();
                vm.ShowAddEmployeeForm = true;
            }
        }
        private void OnCancelAddEmployeeClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is EmployeeViewModel vm)
            {
                vm.ShowAddEmployeeForm = false;
                vm.NewEmployee = new Employee();
            }
        }
        private void OnSaveEmployeeClick(object? sender, RoutedEventArgs e)
        {
            // Implement saving logic here
        }
    }
}
