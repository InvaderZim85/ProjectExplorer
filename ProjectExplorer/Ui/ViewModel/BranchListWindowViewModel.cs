using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectExplorer.Common;
using ProjectExplorer.Models.Data;
using ProjectExplorer.Models.Internal;
using System.Collections.ObjectModel;
using LibGit2Sharp;

namespace ProjectExplorer.Ui.ViewModel;

/// <summary>
/// Interaction logic for <see cref="View.BranchListWindow"/>
/// </summary>
internal partial class BranchListWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the window title
    /// </summary>
    [ObservableProperty]
    private string _windowTitle = "Branches";

    /// <summary>
    /// Gets or sets the info
    /// </summary>
    [ObservableProperty]
    private string _info = string.Empty;

    /// <summary>
    /// Gets or sets the list with the files
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<BranchEntry> _branches = [];

    /// <summary>
    /// Gets or sets the selected branch
    /// </summary>
    [ObservableProperty]
    private BranchEntry? _selectedBranch;

    /// <summary>
    /// Occurs when the user selects another branch
    /// </summary>
    /// <param name="value">The selected branch</param>
    partial void OnSelectedBranchChanged(BranchEntry? value)
    {
        Commits = (value?.Commits ?? []).ToObservableCollection();
    }

    /// <summary>
    /// Gets or sets the list with the commits
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<CommitEntry> _commits = [];

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="project">The project</param>
    public void InitViewModel(ProjectEntry project)
    {
        WindowTitle = $"{project.Name} - Branches";
        Branches = project.Branches.ToObservableCollection();
        Info = $"Project: {project.Name} | Branches: {project.Branches.Count}";
    }

    /// <summary>
    /// Exports the current list as CSV file
    /// </summary>
    [RelayCommand]
    private async Task ExportAsync()
    {
        if (!Branches.Any())
            return;

        try
        {
            await ExportAsCsvAsync(Branches);
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
        if (!Branches.Any())
            return;

        CopyToClipboard(Branches);
    }
}