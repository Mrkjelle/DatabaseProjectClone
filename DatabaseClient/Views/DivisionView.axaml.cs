using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;
using DatabaseClient.Utilities;
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
        if (e.PropertyName == "DivisionID")
        {
            e.Cancel = true;
        }
    }

    private async void OnAssignProjectSubmenuOpened(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem assignMenu && DataContext is DivisionViewModel vm)
        {
            assignMenu.Items.Clear();

            if (DivisionGrid.SelectedItem is not Division division)
            {
                assignMenu.Items.Add(
                    new MenuItem { Header = "No division selected", IsEnabled = false }
                );
                return;
            }

            try
            {
                var repo = new ProjectRepository();
                var orgRepo = new OrgRepository();
                var availableProjects = repo.GetAvailableProjectsForDivision(division.DivisionID);

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
                            repo.AddDivisionToProject(division.DivisionID, proj.ProjectID);

                            AppStatus.ShowMessage?.Invoke(
                                $"Assigned division '{division.DivisionCode}' to project '{proj.ProjectCode}'."
                            );
                        }
                        catch (Exception ex)
                        {
                            AppStatus.ShowMessage?.Invoke($"Error assigning project: {ex.Message}");
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

    private void OnEditDivisionClick(object? sender, RoutedEventArgs e)
    {
        // To be implemented: Edit division functionality
    }

    private void OnDeleteDivisionClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DivisionViewModel vm && DivisionGrid.SelectedItem is Division division)
        {
            vm.DeleteDivision(division);
        }
        else
        {
            AppStatus.ShowMessage?.Invoke("No division selected for deletion.");
        }
    }
}
