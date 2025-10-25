using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DatabaseClient.ViewModels;

namespace DatabaseClient.Views
{
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
    }
}