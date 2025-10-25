using Avalonia.Controls;

namespace DatabaseClient.Views
{
    public partial class LoginView : UserControl
    {
        private readonly LoginViewModel _viewModel = new();
        public LoginView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void OnLoginClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var window = this.VisualRoot as Window;
            _viewModel.DoLogin(window);
        }
    }
}
