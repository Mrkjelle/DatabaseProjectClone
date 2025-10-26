using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using DatabaseClient.ViewModels;

namespace DatabaseClient.Utilities;

public static class GlobalExceptionHandler
{
    private static MainWindowViewModel? _mainWindowViewModel;

    public static void Initialize(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        Dispatcher.UIThread.UnhandledException += OnUiThreadException;
    }

    private static void Report(string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_mainWindowViewModel != null)
            {
                _mainWindowViewModel.StatusMessage = $"Error: {DateTime.Now}: {message}";
            }
        });
    }

    private static void OnUiThreadException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Report($"UI exception: {e.Exception.Message}");
        e.Handled = true; // prevents Avalonia crash popup
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            Report($"Fatal exception: {ex.Message}");
    }

    private static void OnUnobservedTaskException(
        object? sender,
        UnobservedTaskExceptionEventArgs e
    )
    {
        Report($"Background task exception: {e.Exception.Message}");
        e.SetObserved();
    }
}
