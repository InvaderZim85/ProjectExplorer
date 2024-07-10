using ProjectExplorer.Business;
using ProjectExplorer.Helpers;
using ProjectExplorer.Models;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace ProjectExplorer.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    private int _counter = 0;

    /// <summary>
    /// Gets or sets the list with the project directories
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ProjectEntry> _projectDirs = [];

    /// <summary>
    /// Gets or sets the selected project dir
    /// </summary>
    [ObservableProperty]
    private ProjectEntry? _selectedProjectDir;

    [RelayCommand]
    private void OnCounterIncrement()
    {
        Counter++;
    }

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        // Load the projects
        GitHelper.LoadRepoInformation();
        ProjectDirs = SettingsManager.Settings.Projects.ToObservableCollection();
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
        
    }

    [RelayCommand]
    private void BrowseProjectDir()
    {
        //var dialog = new OpenFolderDialog();

        //if (dialog.ShowDialog() != true)
        //    return;

        //// Check if the path already exists
        //if (ProjectDirs.Any(a => a.Equals(dialog.FolderName, StringComparison.OrdinalIgnoreCase)))
        //{
        //    SelectedProjectDir = ProjectDirs.FirstOrDefault(f => f.Equals(dialog.FolderName)) ?? string.Empty;
        //    return;
        //}

        //ProjectDirs.Add(dialog.FolderName);

        //// Save the changes
        //SettingsManager.SaveSettings();
    }
}