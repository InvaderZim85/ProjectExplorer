using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectExplorer.Common;
using ProjectExplorer.Models.Data;
using ProjectExplorer.Models.Internal;
using System.Collections.ObjectModel;

namespace ProjectExplorer.Ui.ViewModel;

/// <summary>
/// Interaction logic for <see cref="View.FileStatusWindow"/>
/// </summary>
internal partial class FileStatusWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the window title
    /// </summary>
    [ObservableProperty]
    private string _windowTitle = "Files";

    /// <summary>
    /// Gets or sets the list with the files
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<FileStatusEntry> _files = [];

    /// <summary>
    /// Gets or sets the info
    /// </summary>
    [ObservableProperty]
    private string _info = string.Empty;

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="project">The project</param>
    public void InitViewModel(ProjectEntry project)
    {
        WindowTitle = $"{project.Name} - Files [{project.StatusInfo}]";
        Files = project.Files.ToObservableCollection();
        Info = $"Project: {project.Name} | Files: {project.Files.Count} | Status: {project.Status} [{project.StatusInfo}]";
    }

    /// <summary>
    /// Exports the current list as CSV file
    /// </summary>
    [RelayCommand]
    private async Task ExportAsync()
    {
        if (!Files.Any())
            return;

        try
        {
            await ExportAsCsvAsync(Files);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
    }

    /// <summary>
    /// Copies the content to the clipboard
    /// </summary>
    [RelayCommand]
    private void CopyToClipboard()
    {
        if (!Files.Any())
            return;

        CopyToClipboard(Files);
    }
}