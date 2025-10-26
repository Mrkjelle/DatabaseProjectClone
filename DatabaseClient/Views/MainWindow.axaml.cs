using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DatabaseClient.Utilities;
using DatabaseClient.ViewModels;
using DatabaseClient.Views;

namespace DatabaseClient.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        var vm = new MainWindowViewModel();
        DataContext = vm;

        GlobalExceptionHandler.Initialize(vm);

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
        DivisionsButton.Click += (_, _) => LoadView(new DivisionView());
        ProjectsButton.Click += (_, _) => LoadView(new ProjectView());
        AssignmentsButton.Click += (_, _) => LoadView(new AssignmentView());
    }

    private void LoadView(UserControl view)
    {
        var content = this.FindControl<ContentControl>("MainContent");
        if (content != null)
            content.Content = view;
    }
}
