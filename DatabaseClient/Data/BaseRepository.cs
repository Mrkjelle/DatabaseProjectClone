using System;
using System.Data;

namespace DatabaseClient.Data;

public abstract class BaseRepository
{
    protected readonly string _primaryConnectionString;
    protected readonly string? _secondaryConnectionString;

    protected BaseRepository(
        string primaryConnectionString,
        string? secondaryConnectionString = null
    )
    {
        _primaryConnectionString =
            primaryConnectionString
            ?? throw new ArgumentNullException(nameof(primaryConnectionString));
        _secondaryConnectionString = secondaryConnectionString;
    }

    protected void EnsureConnection()
    {
        if (string.IsNullOrWhiteSpace(_primaryConnectionString))
        {
            throw new InvalidOperationException("Connection string is not initialized.");
        }
    }

    protected void EnsureBothConnections()
    {
        if (
            string.IsNullOrWhiteSpace(_primaryConnectionString)
            || string.IsNullOrWhiteSpace(_secondaryConnectionString)
        )
        {
            throw new InvalidOperationException(
                "One or both connection strings are not initialized."
            );
        }
    }

    protected virtual void LogError(Exception ex)
    {
        Console.Error.WriteLine($"[{DateTime.UtcNow}] Error: {ex.Message}");
    }

    public void WarmUp()
    {
        SqlServerConnection.PrewarmConnection(_primaryConnectionString);
    }
}
