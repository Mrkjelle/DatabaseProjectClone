using System;
using System.Collections.ObjectModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;

namespace DatabaseClient.ViewModels;

public class DivisionViewModel
{
    public ObservableCollection<Division> Divisions { get; } = new();

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
}
