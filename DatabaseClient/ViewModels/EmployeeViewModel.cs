using System;
using System.Collections.ObjectModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;

namespace DatabaseClient.ViewModels;

public class EmployeeViewModel
{
    public ObservableCollection<Employee> Employees { get; } = new();

    public EmployeeViewModel()
    {
        LoadEmployees();
    }

    private void LoadEmployees()
    {
        try
        {
            var repo = new OrgRepository();
            var employees = repo.GetEmployees();

            Employees.Clear();
            foreach (var emp in employees)
            {
                Console.WriteLine($"Loading employee: {emp.FirstName} {emp.LastName}");
                Employees.Add(emp);
            }
            Console.WriteLine($"Total employees loaded: {Employees.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading employees: {ex.Message}");
        }
    }
}
