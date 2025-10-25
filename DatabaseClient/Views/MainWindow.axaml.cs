using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DatabaseClient.Views;

namespace DatabaseClient.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        ExitButton.Click += (_, _) =>
        {
            if (
                Application.Current?.ApplicationLifetime
                is IClassicDesktopStyleApplicationLifetime lifetime
            )
            {
                var login = new LoginWindow();
                lifetime.MainWindow = login;
                login.Show();

                this.Close();
            }
        };
    }
}
