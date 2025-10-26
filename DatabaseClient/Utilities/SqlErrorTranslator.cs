using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DatabaseClient.Utilities;

public static class SqlErrorTranslator
{
    public static readonly Dictionary<string, string> _translations = new()
    {
        // constraint names
        { "CK_Email_Valid", "Ensure you use a valid email address." },
        { "UQ_Email", "This email address is already registered to another employee." },
        { "FK_Employee_Division", "Please select a valid division before saving the employee." },
        { "UQ_EmployeeNO", "That employee number already exists in the system." },
        { "SqlDateTime overflow", "Please select a valid start date for the employee." },
        { "CK_HireDate_NotFuture", "Hire date cannot be in the future." },
        { "CK_EmployeeNO_Format", "Employee number cannot be empty or contain spaces." },
        { "CK_Name_NotEmpty", "First and last name cannot be empty or only spaces." },
        // general messages
        { "FOREIGN KEY constraint", "You're referencing something that doesn't exist." },
        { "CHECK constraint", "The provided data failed one of the validation rules." },
        { "UNIQUE KEY constraint", "Duplicate value detected — this must be unique." },
    };

    public static string Translate(string sqlEx)
    {
        if (string.IsNullOrWhiteSpace(sqlEx))
        {
            return "An unknown database error occurred.";
        }

        foreach (var translation in _translations)
        {
            if (sqlEx.Contains(translation.Key, StringComparison.OrdinalIgnoreCase))
            {
                return translation.Value;
            }
        }

        return "A database error occurred.";
    }
}
