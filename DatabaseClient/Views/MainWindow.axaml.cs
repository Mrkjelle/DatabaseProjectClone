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
        this.Opened += (_, _) => LoadView(new EmployeeView());
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
        EmployeesButton.Click += (_, _) => LoadView(new EmployeeView());
    }

    private void LoadView(UserControl view)
    {
        var content = this.FindControl<ContentControl>("MainContent");
        if (content != null)
            content.Content = view;
    }
}
