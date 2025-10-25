using System;
using System.ComponentModel.Design.Serialization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DatabaseClient.Views;

namespace DatabaseClient;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        _ = Task.Run(PreWarmConnections);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // don’t set desktop.MainWindow
            var login = new LoginWindow();
            login.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void PreWarmConnections()
    {
        try
        {
            var orgRepo = new Data.OrgRepository();
            var projectRepo = new Data.ProjectRepository();

            orgRepo.WarmUp();
            projectRepo.WarmUp();

            Console.WriteLine("[INIT] Connection pools ready");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[INIT ERROR] Failed to pre-warm connections: {ex.Message}");
        }
    }
}
