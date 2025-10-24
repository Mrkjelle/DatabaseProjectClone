using Avalonia;
using Avalonia.Controls;
using DatabaseClient.ViewModels;

namespace DatabaseClient.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
