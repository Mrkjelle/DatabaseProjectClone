using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;

namespace DatabaseClient.ViewModels;

public class EmployeeViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Employee> Employees { get; } = new();
    public ObservableCollection<Division> Divisions { get; } = new();
    private Division? _selectedDivision;
    public Division? SelectedDivision
    {
        get => _selectedDivision;
        set
        {
            if (_selectedDivision != value)
            {
                _selectedDivision = value;
                if (NewEmployee != null)
                {
                    NewEmployee.DivisionID = value?.DivisionID ?? 0;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(NewEmployee));
            }
        }
    }
    private bool _showAddEmployeeForm;
    public bool ShowAddEmployeeForm
    {
        get => _showAddEmployeeForm;
        set
        {
            if (_showAddEmployeeForm != value)
            {
                _showAddEmployeeForm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowEmployeeGrid));
            }
        }
    }
    public bool ShowEmployeeGrid => !ShowAddEmployeeForm;

    public Employee NewEmployee { get; set; } = new Employee();

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public EmployeeViewModel()
    {
        LoadEmployees();
        LoadDivisions();
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

    private void LoadDivisions()
    {
        try
        {
            var repo = new OrgRepository();
            var divisions = repo.GetDivisions();

            foreach (var div in divisions)
            {
                Divisions.Add(div);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading divisions: {ex.Message}");
        }
    }
}
