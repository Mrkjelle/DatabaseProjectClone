using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using DatabaseClient.ViewModels;

namespace DatabaseClient.Views;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
        DataContext = new ProjectViewModel();

        this.AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ProjectViewModel vm)
        {
            var sorted = vm.Projects.OrderBy(proj => proj.ProjectName).ToList();
            vm.Projects.Clear();
            foreach (var proj in sorted)
            {
                vm.Projects.Add(proj);
            }
        }
    }

    private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName == "ProjectID" || e.PropertyName == "DivisionID")
        {
            e.Cancel = true;
        }

        if (e.PropertyName == "EndDate")
        {
            var textColumn = new DataGridTextColumn
            {
                Header = "End Date",
                Binding = new Binding("EndDate")
                {
                    Converter = new NullToOnGoingConverter(),
                    Mode = BindingMode.OneWay,
                },
            };
            e.Column = textColumn;
        }
        if (e.PropertyName == "StartDate")
        {
            var dateColumn = new DataGridTextColumn
            {
                Header = "Start Date",
                Binding = new Avalonia.Data.Binding("StartDate")
                {
                    Converter = new Utilities.DateOnlyConverter(),
                },
            };
            e.Column = dateColumn;
        }
    }

    public class NullToOnGoingConverter : IValueConverter
    {
        public object? Convert(
            object? value,
            Type targetType,
            object? parameter,
            System.Globalization.CultureInfo culture
        )
        {
            if (value == null || value == DBNull.Value)
                return "In Progress";

            if (value is DateTime dt)
            {
                if (dt == DateTime.MinValue)
                    return "In Progress";
                return dt.ToString("yyyy-MM-dd");
            }
            return value;
        }

        public object? ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            System.Globalization.CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
