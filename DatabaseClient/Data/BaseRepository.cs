using System;
using System.Data;
using DatabaseClient.Utilities;

namespace DatabaseClient.Data;

public abstract class BaseRepository
{
    protected readonly string _primaryConnectionString;

    protected BaseRepository(string primaryConnectionString)
    {
        _primaryConnectionString =
            primaryConnectionString
            ?? throw new ArgumentNullException(nameof(primaryConnectionString));
    }

    protected void EnsureConnection()
    {
        if (string.IsNullOrWhiteSpace(_primaryConnectionString))
        {
            throw new InvalidOperationException("Connection string is not initialized.");
        }
    }

    protected virtual void LogError(Exception ex)
    {
        AppStatus.ShowMessage?.Invoke($"Error: {ex.Message}");
        Console.Error.WriteLine($"[{DateTime.UtcNow}] Error: {ex.Message}");
    }

    public void WarmUp()
    {
        SqlServerConnection.PrewarmConnection(_primaryConnectionString);
    }
}
