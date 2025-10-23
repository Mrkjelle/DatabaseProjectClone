using System;
using System.Data;

namespace DatabaseClient.Data;

public abstract class BaseRepository
{
    protected readonly string _connectionString;

    protected BaseRepository(string connectionString)
    {
        _connectionString =
            connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    protected void EnsureConnection()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string is not initialized.");
        }
    }

    protected virtual void LogError(Exception ex)
    {
        Console.Error.WriteLine($"[{DateTime.UtcNow}] Error: {ex.Message}");
    }
}
