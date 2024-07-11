using MahApps.Metro.Controls;
using ProjectExplorer.Models.Data;
using ProjectExplorer.Ui.ViewModel;
using System.Windows;

namespace ProjectExplorer.Ui.View;

/// <summary>
/// Interaction logic for FileStatusWindow.xaml
/// </summary>
public partial class FileStatusWindow : MetroWindow
{
    /// <summary>
    /// Contains the project
    /// </summary>
    private readonly ProjectEntry _project;

    /// <summary>
    /// Creates a new instance of the <see cref="FileStatusWindow"/>
    /// </summary>
    /// <param name="project">The project which should be shown</param>
    public FileStatusWindow(ProjectEntry project)
    {
        InitializeComponent();
        _project = project;
    }

    /// <summary>
    /// Occurs when the window was loaded
    /// </summary>
    /// <param name="sender">The <see cref="FileStatusWindow"/></param>
    /// <param name="e">The event arguments</param>
    private void FileStatusWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is FileStatusWindowViewModel viewModel)
            viewModel.InitViewModel(_project);
    }

    /// <summary>
    /// Occurs when the user hits the close button (bottom right)
    /// </summary>
    /// <param name="sender">The close button</param>
    /// <param name="e">The event arguments</param>
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}