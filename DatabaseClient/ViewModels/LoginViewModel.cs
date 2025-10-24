using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DatabaseClient.Views;
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
            LoginCommand = ReactiveCommand.Create(DoLogin);
        }

        private void DoLogin()
        {
            try
            {
                if (
                    Application.Current?.ApplicationLifetime
                    is IClassicDesktopStyleApplicationLifetime lifetime
                )
                {
                    Console.WriteLine("Creating MainWindow...");
                    var main = new Views.MainWindow();
                    Console.WriteLine("MainWindow constructed successfully.");

                    lifetime.MainWindow = main;
                    main.Show();
                    Console.WriteLine("MainWindow shown.");

                    foreach (var w in lifetime.Windows.ToList())
                    {
                        if (w is Views.LoginWindow)
                            w.Close();
                    }
                    Console.WriteLine("Login window closed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during DoLogin: " + ex);
            }
        }
    }
}
