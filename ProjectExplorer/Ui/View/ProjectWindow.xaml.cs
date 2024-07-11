using System.Windows;
using MahApps.Metro.Controls;
using ProjectExplorer.Models.Data;
using ProjectExplorer.Ui.ViewModel;

namespace ProjectExplorer.Ui.View;

/// <summary>
/// Interaction logic for ProjectWindow.xaml
/// </summary>
public partial class ProjectWindow : MetroWindow
{
    /// <summary>
    /// Gets the created project
    /// </summary>
    public ProjectEntry Project { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="ProjectWindow"/>
    /// </summary>
    /// <param name="project">The project which should be edited</param>
    public ProjectWindow(ProjectEntry? project = null)
    {
        InitializeComponent();
        Project = project ?? new ProjectEntry();
    }

    /// <summary>
    /// Occurs when the window was loaded
    /// </summary>
    /// <param name="sender">The <see cref="ProjectWindow"/></param>
    /// <param name="e">The event arguments</param>
    private void ProjectWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProjectWindowViewModel viewModel)
        {
            viewModel.InitViewModel(Project, result => { DialogResult = result; });
        }
    }
}