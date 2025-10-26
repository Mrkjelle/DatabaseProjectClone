using System;
using System.Collections.ObjectModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;
using DatabaseClient.Utilities;

namespace DatabaseClient.ViewModels;

public class DivisionViewModel
{
    public ObservableCollection<Division> Divisions { get; } = new();
    public Division? SelectedDivision { get; set; }

    public DivisionViewModel()
    {
        LoadDivisions();
    }

    private void LoadDivisions()
    {
        try
        {
            var repo = new OrgRepository();
            var divisions = repo.GetDivisions();

            Divisions.Clear();
            foreach (var div in divisions)
            {
                Console.WriteLine($"Loading division: {div.DivisionName}");
                Divisions.Add(div);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading divisions: {ex.Message}");
        }
    }

    public void DeleteDivision(Division division)
    {
        try
        {
            var repo = new OrgRepository();
            repo.DeleteDivisionAndDependencies(division.DivisionID);

            AppStatus.ShowMessage?.Invoke(
                $"Deleted division '{division.DivisionCode}' and its dependencies."
            );
            Divisions.Remove(division);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
