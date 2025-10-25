using System.Collections.ObjectModel;
using Avalonia.Controls;
using DatabaseClient.Models;
using DatabaseClient.Repositories;

namespace DatabaseClient.ViewModels;

public class EmployeeViewModel
{
    public ObservableCollection<Employee> Employees { get; set; }

    public EmployeeViewModel()
    {
        LoadEmployees();
    }

    private void LoadEmployees()
    {
        var employeeData = OrgRepository.GetEmployees();
        Employees = new ObservableCollection<Employee>(employeeData);
    }
}
