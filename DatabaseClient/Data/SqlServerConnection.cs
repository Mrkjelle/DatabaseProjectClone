using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DatabaseClient.Data;

public static class SqlServerConnection
{
    public static void ExecuteStoredProcedureSimple(
        string connectionString,
        string storedProcedureName,
        params SqlParameter[] parameters
    )
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            if (parameters?.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            // Log exception (not implemented here)
            throw new DataException(
                $"Error executing stored procedure '{storedProcedureName}' via {nameof(ExecuteStoredProcedureSimple)}.",
                ex
            );
        }
    }

    public static DataTable ExecuteStoredProcedureTable(
        string connectionString,
        string storedProcedureName,
        params SqlParameter[] parameters
    )
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            if (parameters?.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            var adapter = new SqlDataAdapter(command);
            var table = new DataTable();

            connection.Open();
            adapter.Fill(table);

            return table;
        }
        catch (SqlException ex)
        {
            // Log exception (not implemented here)
            throw new DataException(
                $"Error executing stored procedure '{storedProcedureName}' via {nameof(ExecuteStoredProcedureTable)}.",
                ex
            );
        }
    }

    public static SqlDataReader ExecuteStoredProcedureReader(
        string connectionString,
        string storedProcedureName,
        params SqlParameter[] parameters
    )
    {
        try
        {
            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            if (parameters?.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch (SqlException ex)
        {
            // Log exception (not implemented here)
            throw new DataException(
                $"Error executing stored procedure '{storedProcedureName}' via {nameof(ExecuteStoredProcedureReader)}.",
                ex
            );
        }
    }

    public static object? ExecuteStoredProcedureScalar(
        string connectionString,
        string storedProcedureName,
        params SqlParameter[] parameters
    )
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            if (parameters?.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            // Log exception (not implemented here)
            throw new DataException(
                $"Error executing stored procedure '{storedProcedureName}' via {nameof(ExecuteStoredProcedureScalar)}.",
                ex
            );
        }
    }
    public static void PrewarmConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        try
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("[Init] Connection pre-warmed successfully.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"[Init Error] Failed to pre-warm connection: {ex.Message}");
            throw;
        }
    }
}
