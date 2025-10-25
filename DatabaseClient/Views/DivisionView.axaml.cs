using Avalonia.Controls;

namespace DatabaseClient.Views;

public partial class DivisionView : UserControl
{
    public DivisionView()
    {
        InitializeComponent();
        DataContext = new ViewModels.DivisionViewModel();
    }
}
