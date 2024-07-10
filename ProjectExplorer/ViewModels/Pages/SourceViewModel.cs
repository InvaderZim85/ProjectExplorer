using Microsoft.Win32;
using ProjectExplorer.Business;
using ProjectExplorer.Helpers;
using ProjectExplorer.Models;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace ProjectExplorer.ViewModels.Pages;

/// <summary>
/// Interaction logic for <see cref="Views.Pages.SourcePage"/>
/// </summary>
public partial class SourceViewModel : ObservableObject, INavigationAware
{
    /// <summary>
    /// Gets or sets the list with the sources
    /// </summary>
    [ObservableProperty] 
    private ObservableCollection<ProjectEntry> _sources = [];

    /// <summary>
    /// Gets or sets the selected source
    /// </summary>
    [ObservableProperty] 
    private ProjectEntry? _selectedSource;

    /// <summary>
    /// Occurs when the user selects another source
    /// </summary>
    /// <param name="value">The selected project entry</param>
    partial void OnSelectedSourceChanged(ProjectEntry? value)
    {
        ButtonDeleteEnabled = value != null;
    }

    /// <summary>
    /// Gets or sets the value which indicates if the deleted button is enabled
    /// </summary>
    [ObservableProperty] 
    private bool _buttonDeleteEnabled;

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        Sources = SettingsManager.Settings.Projects.ToObservableCollection();
        SelectedSource = Sources.FirstOrDefault();
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
        // Save the current values
        SettingsManager.Settings.Projects = Sources.ToList();
        SettingsManager.SaveSettings();
    }

    /// <summary>
    /// Browses for a new folder and adds it to the source list
    /// </summary>
    [RelayCommand]
    private void BrowseFolder()
    {
        var dialog = new OpenFolderDialog
        {
            Multiselect = false
        };

        if (dialog.ShowDialog() != true)
            return;

        //Sources.Add(dialog.FolderName);
        //SelectedSource = dialog.FolderName;
    }

    /// <summary>
    /// Occurs when the user hits the delete button
    /// </summary>
    [RelayCommand]
    private void DeleteSelectedSource()
    {

    }
}