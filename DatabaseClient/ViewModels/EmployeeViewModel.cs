using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;
using DatabaseClient.Utilities;
using Microsoft.Data.SqlClient;

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
                    NewEmployee.DivisionID = value?.DivisionID ?? 0;

                OnPropertyChanged();
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
    public void DeleteEmployee(Employee emp)
    {
        try
        {
            var repo = new OrgRepository();
            repo.DeleteEmployee(emp.EmpID);
            Employees.Remove(emp);
            AppStatus.ShowMessage?.Invoke(
                $"Employee {emp.FirstName} {emp.LastName} deleted successfully."
            );
        }
        catch (SqlException sqlEx)
        {
            var userMessage = SqlErrorTranslator.Translate(sqlEx.Message);
            AppStatus.ShowMessage?.Invoke($"Error deleting employee: {userMessage}");
            Console.WriteLine($"SQL Error deleting employee: {sqlEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting employee: {ex.Message}");
        }
    }
}
