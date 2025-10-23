using System.Data;
using Microsoft.Data.SqlClient;

namespace DatabaseClient.Data;

public static class SqlServerConnection
{
    public static DataTable ExecuteStoredProcedure(
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
            throw new DataException("Stored procedure execution failed.", ex);
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
            throw new DataException("Stored procedure reader execution failed.", ex);
        }
    }
}
