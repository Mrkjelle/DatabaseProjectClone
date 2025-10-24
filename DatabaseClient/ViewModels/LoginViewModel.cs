using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DatabaseClient.Views;
using MessageBox.Avalonia;
using ReactiveUI;

namespace DatabaseClient.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        private string _username = string.Empty;
        private string _password = string.Empty;

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }

        public LoginViewModel()
        {
            // keep all command notifications on the Avalonia dispatcher
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            LoginCommand = ReactiveCommand.Create(
                DoLogin,
                outputScheduler: RxApp.MainThreadScheduler
            );
        }

        private void DoLogin()
        {
            // very basic credential check
            if (Username == "admin" && Password == "admin")
            {
                // open the main window
                if (
                    Application.Current?.ApplicationLifetime
                    is IClassicDesktopStyleApplicationLifetime lifetime
                )
                {
                    var main = new Views.MainWindow();
                    main.Show();

                    // close the login window
                    var login = lifetime.Windows.FirstOrDefault(w => w is Views.LoginWindow);
                    login?.Close();
                }
            }
            else
            {
                // feedback: invalid credentials
                MessageBoxManager
                    .GetMessageBoxStandardWindow("Access Denied", "Invalid username or password.")
                    .Show();
            }
        }
    }
}
