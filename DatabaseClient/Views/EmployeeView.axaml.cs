using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;
using DatabaseClient.Utilities;
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
                vm.NewEmployee = new Employee { HireDate = DateTime.Today };
                vm.ShowAddEmployeeForm = true;

                HireDatePicker.SelectedDate = new DateTimeOffset(DateTime.Today);
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
            if (DataContext is EmployeeViewModel vm)
            {
                try
                {
                    if (HireDatePicker.SelectedDate is DateTimeOffset dto)
                    {
                        vm.NewEmployee.HireDate = dto.DateTime;
                    }
                    if (vm.IsEditing)
                    {
                        var repo = new OrgRepository();
                        repo.UpdateEmployee(vm.NewEmployee);
                        AppStatus.ShowMessage?.Invoke(
                            $"Employee {vm.NewEmployee.FirstName} updated successfully."
                        );
                    }
                    else
                    {
                        var repo = new OrgRepository();
                        repo.AddEmployee(vm.NewEmployee);
                        AppStatus.ShowMessage?.Invoke(
                            $"Employee {vm.NewEmployee.FirstName} added successfully."
                        );
                    }

                    vm.ShowAddEmployeeForm = false;
                    vm.IsEditing = false;
                    vm.LoadEmployees();
                }
                catch (Exception ex)
                {
                    AppStatus.ShowMessage?.Invoke(
                        $"Error saving employee: {SqlErrorTranslator.Translate(ex.Message)}"
                    );
                }
            }
        }

        private void OnEditEmployeeClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is EmployeeViewModel vm && EmployeeGrid.SelectedItem is Employee emp)
            {
                vm.EditEmployee(emp);
                AppStatus.ShowMessage?.Invoke($"Editing employee: {emp.FirstName} {emp.LastName}");
            }
            else
            {
                AppStatus.ShowMessage?.Invoke("No employee selected for editing.");
            }
        }

        private void OnDeleteEmployeeClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is EmployeeViewModel vm && EmployeeGrid.SelectedItem is Employee emp)
            {
                vm.DeleteEmployee(emp);
            }
            else
            {
                AppStatus.ShowMessage?.Invoke("No employee selected for deletion.");
            }
        }

        private async void OnAssignProjectSubmenuOpened(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem assignMenu && DataContext is EmployeeViewModel vm)
            {
                assignMenu.Items.Clear();

                if (EmployeeGrid.SelectedItem is not Employee emp)
                {
                    assignMenu.Items.Add(
                        new MenuItem { Header = "No employee selected", IsEnabled = false }
                    );
                    return;
                }

                try
                {
                    var repo = new ProjectRepository();
                    var availableProjects = repo.GetAssignableProjectsForEmployee(emp.EmpID);

                    if (availableProjects.Count == 0)
                    {
                        assignMenu.Items.Add(
                            new MenuItem { Header = "No available projects", IsEnabled = false }
                        );
                        return;
                    }

                    foreach (var proj in availableProjects)
                    {
                        var item = new MenuItem
                        {
                            Header = $"{proj.ProjectCode} - {proj.ProjectName}",
                            Tag = proj.ProjectID,
                        };

                        item.Click += async (_, _) =>
                        {
                            try
                            {
                                var dialog = new AssignEmployeeDialog();
                                var result = await dialog.ShowDialog<bool>(
                                    (Window)this.VisualRoot!
                                );

                                if (!result)
                                {
                                    AppStatus.ShowMessage?.Invoke("Assignment cancelled.");
                                    return;
                                }

                                var role = dialog.Role;
                                var hours = dialog.HoursWorked;

                                var repo = new ProjectRepository();
                                repo.AddEmployeeToProject(emp.EmpID, proj.ProjectID, role, hours);

                                AppStatus.ShowMessage?.Invoke(
                                    $"Assigned {emp.FirstName} {emp.LastName} to {proj.ProjectCode} as {role} ({hours}h)."
                                );
                            }
                            catch (Exception ex)
                            {
                                AppStatus.ShowMessage?.Invoke(
                                    $"Error assigning project: {ex.Message}"
                                );
                            }
                        };

                        assignMenu.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    AppStatus.ShowMessage?.Invoke($"Error loading projects: {ex.Message}");
                }
            }
        }
    }
}
