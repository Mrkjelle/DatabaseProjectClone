using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DatabaseClient.Views
{
    public partial class AssignEmployeeDialog : Window
    {
        public string Role => RoleTextBox.Text ?? "";
        public decimal HoursWorked
        {
            get
            {
                if (decimal.TryParse(HoursTextBox.Text, out var val))
                    return val;
                return 0;
            }
        }

        public AssignEmployeeDialog()
        {
            InitializeComponent();

            OkButton.Click += (_, _) => Close(true);
            CancelButton.Click += (_, _) => Close(false);
        }
    }
}
