using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ProjectExplorer.Business;
using ProjectExplorer.Models.Data;

namespace ProjectExplorer.Ui.ViewModel;

/// <summary>
/// Interaction logic for <see cref="View.ProjectWindow"/>
/// </summary>
internal partial class ProjectWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The action to close the window
    /// </summary>
    private Action<bool>? _closeWindow;

    /// <summary>
    /// Gets or sets the project
    /// </summary>
    [ObservableProperty]
    private ProjectEntry _project = new();

    /// <summary>
    /// Gets or sets the window title
    /// </summary>
    [ObservableProperty]
    private string _windowTitle = "New project";

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="project">The project</param>
    /// <param name="closeWindow">The window</param>
    public void InitViewModel(ProjectEntry project, Action<bool> closeWindow)
    {
        Project = project;
        _closeWindow = closeWindow;

        WindowTitle = project.Id == 0 ? "New project" : "Edit project";
    }

    /// <summary>
    /// Browse for a solution
    /// </summary>
    [RelayCommand]
    private void BrowseSolution()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Solution file (*.sln)|*.sln"
        };

        if (dialog.ShowDialog() != true)
            return;

        Project.Path = dialog.FileName;
    }

    /// <summary>
    /// Validates the input
    /// </summary>
    [RelayCommand]
    private async Task ValidateAsync()
    {
        if (string.IsNullOrEmpty(Project.Name) || string.IsNullOrEmpty(Project.Path))
        {
            ShowInfoMessage("Name and / or path is missing.");
            return;
        }

        using var manager = new DataManager();
        if (await manager.ProjectExistsAsync(Project.Id, Project.Path))
        {
            ShowInfoMessage("A project with the specified path already exists.");
            return;
        }

        _closeWindow?.Invoke(true);
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    [RelayCommand]
    private void Close()
    {
        _closeWindow?.Invoke(false);
    }
}