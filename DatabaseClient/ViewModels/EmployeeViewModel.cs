using System;
using System.Collections.ObjectModel;
using DatabaseClient.Data;
using DatabaseClient.Models.Org;

namespace DatabaseClient.ViewModels;

public class EmployeeViewModel
{
    public ObservableCollection<Employee> Employees { get; set; } = new();

    public EmployeeViewModel()
    {
        LoadEmployees();
    }

    private void LoadEmployees()
    {
        OrgRepository orgRepository = new();
        var employees = orgRepository.GetEmployees();
        Employees = new ObservableCollection<Employee>(employees);
    }
}
