using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using DatabaseClient.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace DatabaseClient.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public void DoLogin(Window currentWindow)
        {
            if (Username == "admin" && Password == "admin")
            {
                if (
                    Application.Current?.ApplicationLifetime
                    is IClassicDesktopStyleApplicationLifetime lifetime
                )
                {
                    var main = new MainWindow();
                    lifetime.MainWindow = main;
                    main.Show();
                }
                currentWindow.Close();
            }
            else
            {
                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Access Denied",
                    "Invalid Credentials",
                    ButtonEnum.Ok,
                    Icon.Error
                );
                _ = msgBox.ShowAsync();
            }
        }
    }
}
