using System.ComponentModel;
using DatabaseClient.Utilities;
using Microsoft.VisualBasic;

namespace DatabaseClient.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public MainWindowViewModel()
    {
        AppStatus.ShowMessage = message => StatusMessage = message;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
