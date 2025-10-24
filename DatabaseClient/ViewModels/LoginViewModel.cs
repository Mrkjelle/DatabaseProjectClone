using System;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DatabaseClient.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
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
            if (Username == "admin" && Password == "admin")
            {
                if (
                    Application.Current?.ApplicationLifetime
                    is IClassicDesktopStyleApplicationLifetime lifetime
                )
                {
                    var main = new Views.MainWindow();
                    main.Show();

                    var login = lifetime.Windows.FirstOrDefault(w => w is Views.LoginWindow);
                    login?.Close();
                }
            }
            else
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandard(
                    new MessageBoxStandardParams
                    {
                        ContentTitle = "Access Denied",
                        ContentMessage = "Invalid username or password.",
                        ButtonDefinitions = ButtonEnum.Ok,
                        Icon = Icon.Error,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    }
                );

                messageBox.ShowAsync();
            }
        }
    }
}
