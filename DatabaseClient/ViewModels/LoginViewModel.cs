using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;
using DatabaseClient.Views;
using Avalonia;

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
            // later you'll swap windows here
            var main = new Views.MainWindow();
            main.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Views.LoginWindow)
                    w.Close();
            }
        }
    }
}