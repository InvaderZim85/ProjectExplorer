using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using ProjectExplorer.Business;
using ProjectExplorer.Common;
using ProjectExplorer.Common.Enums;
using ProjectExplorer.Models.Data;
using ProjectExplorer.Ui.View;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Timer = System.Timers.Timer;

namespace ProjectExplorer.Ui.ViewModel;

/// <summary>
/// Interaction logic for <see cref="View.MainWindow"/>
/// </summary>
internal partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The instance for the interaction with the data
    /// </summary>
    private readonly DataManager _manager = new();

    /// <summary>
    /// Contains the check timer
    /// </summary>
    private Timer? _checkTimer;

    /// <summary>
    /// Contains the date of the last branch check
    /// </summary>
    private DateTime _lastCheck;

    /// <summary>
    /// Contains the current version
    /// </summary>
    private readonly Version _version = Assembly.GetExecutingAssembly().GetName().Version ?? new();

    #region Various
    /// <summary>
    /// Gets or sets the value which indicates whether the "open in visual studio" option is enabled
    /// </summary>
    [ObservableProperty]
    private bool _openInVisualStudioEnabled;

    /// <summary>
    /// Gets or sets the value which indicates whether the "open in visual studio code" option is enabled
    /// </summary>
    [ObservableProperty]
    private bool _openInVisualStudioCodeEnabled;

    /// <summary>
    /// Gets or sets the value which indicates if the timer is enabled
    /// </summary>
    [ObservableProperty]
    private bool _timerEnabled;
    
    /// <summary>
    /// Occurs when the user activates the timer
    /// </summary>
    /// <param name="value">The new value</param>
    partial void OnTimerEnabledChanged(bool value)
    {
        if (value)
        {
            _checkTimer = new Timer(TimeSpan.FromMinutes(CheckTime));

            _checkTimer.Elapsed += (_, _) =>
            {
                LoadBranchInfo(true);
            };

            LoadBranchInfo(true);

            _checkTimer.Start();
        }
        else
        {
            _checkTimer?.Stop();
            _checkTimer?.Dispose();

            LoadBranchInfo(true);
        }
    }

    /// <summary>
    /// Gets or sets the check time
    /// </summary>
    [ObservableProperty]
    private int _checkTime = 5;

    /// <summary>
    /// Gets or sets the window title
    /// </summary>
    [ObservableProperty]
    private string _windowTitle = "Branch Info";

    /// <summary>
    /// Gets or sets the last check info
    /// </summary>
    [ObservableProperty]
    private string _lastCheckInfo = "Last check: /";

    /// <summary>
    /// Gets or sets the info (status bar)
    /// </summary>
    [ObservableProperty]
    private string _info = string.Empty;

    #endregion

    #region Projects
    /// <summary>
    /// Gets or sets the list with the projects
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ProjectEntry> _projects = [];

    /// <summary>
    /// Gets or sets the selected project
    /// </summary>
    [ObservableProperty]
    private ProjectEntry? _selectedProject;

    /// <summary>
    /// Occurs when the value of <see cref="SelectedProject"/> was changed
    /// </summary>
    /// <param name="value">The new value</param>
    partial void OnSelectedProjectChanged(ProjectEntry? value)
    {
        OptionsEnabled = value != null;
    }

    /// <summary>
    /// Gets or sets the project filter
    /// </summary>
    [ObservableProperty]
    private string _filterProject = string.Empty;

    /// <summary>
    /// Occurs when the value of <see cref="FilterProject"/> was changed
    /// </summary>
    /// <param name="value">The new value</param>
    partial void OnFilterProjectChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            FilterList();
    }

    /// <summary>
    /// Gets or sets the value which indicates whether the options are enabled
    /// </summary>
    [ObservableProperty]
    private bool _optionsEnabled;
    #endregion

    /// <summary>
    /// Init the view model
    /// </summary>
    public async void InitViewModel()
    {
        // Load the settings
        var vsPath = ConfigManager.LoadValue(ConfigKey.VisualStudioPath, string.Empty);
        var vscPath = ConfigManager.LoadValue(ConfigKey.VisualStudioCodePath, string.Empty);

        OpenInVisualStudioEnabled = File.Exists(vsPath);
        OpenInVisualStudioCodeEnabled = File.Exists(vscPath);

        // Load the projects
        await LoadProjectsAsync();
    }

    /// <summary>
    /// Loads the branch info
    /// </summary>
    /// <param name="catchError"><see langword="true"/> to catch the error (silent), otherwise <see langword="false"/></param>
    private void LoadBranchInfo(bool catchError)
    {
        try
        {
            foreach (var project in _manager.Projects)
            {
                GitHelper.LoadBranchInformation(project);
            }

            _lastCheck = DateTime.Now;

            SetWindowTitle();
        }
        catch (Exception ex)
        {
            if (catchError)
                LogError(ex);
            else
                throw;
        }
    }

    /// <summary>
    /// Filters the project list
    /// </summary>
    [RelayCommand]
    private void FilterList()
    {
        Projects = (string.IsNullOrEmpty(FilterProject)
                ? _manager.Projects
                : _manager.Projects.Where(w => w.Name.Contains(FilterProject, StringComparison.OrdinalIgnoreCase) ||
                                               w.Path.Contains(FilterProject, StringComparison.OrdinalIgnoreCase) ||
                                               w.GitFolder.Contains(FilterProject, StringComparison.OrdinalIgnoreCase)))
            .ToObservableCollection();

        SelectedProject = Projects.FirstOrDefault();
    }

    /// <summary>
    /// Sets the window title
    /// </summary>
    private void SetWindowTitle()
    {
        WindowTitle = TimerEnabled
            ? $"Project Explorer - v{_version} [auto refresh]"
            : $"Project Explorer - v{_version}";

        LastCheckInfo = $"Last check: {_lastCheck:yyyy-MM-dd HH:mm:ss}";
        Info = $"Project Explorer - v{_version} - {LastCheckInfo}";
    }

    #region Commands - Projects
    /// <summary>
    /// Loads the projects
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task LoadProjectsAsync()
    {
        try
        {
            await _manager.LoadProjectsAsync();

            FilterList();

            _lastCheck = DateTime.Now;

            SetWindowTitle();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Load);
        }
    }

    /// <summary>
    /// Adds a new project
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task AddProjectAsync()
    {
        var dialog = new ProjectWindow
        {
            Owner = GetMainWindow()
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            await _manager.AddProjectAsync(dialog.Project);

            // Reset the filter
            FilterProject = string.Empty;

            FilterList();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
    }

    /// <summary>
    /// Edits the selected project
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task EditProjectAsync()
    {
        if (SelectedProject == null)
            return;

        var dialog = new ProjectWindow(SelectedProject)
        {
            Owner = GetMainWindow()
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            await _manager.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
    }

    /// <summary>
    /// Deletes the selected project
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task DeleteProjectAsync()
    {
        if (SelectedProject == null)
            return;

        if (await ShowQuestionAsync("Delete", $"Do you want to delete the project '{SelectedProject.Name}'?", "Yes",
                "No") != MessageDialogResult.Affirmative)
            return;

        try
        {
            await _manager.DeleteProjectAsync(SelectedProject);

            SelectedProject = null;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Delete);
        }
    }

    /// <summary>
    /// Opens the selected project in the desired program
    /// </summary>
    /// <param name="program">The name of the program</param>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task OpenProjectIn(string program)
    {
        if (SelectedProject == null)
            return;

        try
        {
            switch (program)
            {
                case "VisualStudio":
                    var vsPath = ConfigManager.LoadValue(ConfigKey.VisualStudioPath, string.Empty);
                    if (!File.Exists(vsPath))
                    {
                        await ShowMessageAsync("Error", "The path of Visual Studio is not set.");
                        return;
                    }

                    Helper.OpenIn(vsPath, SelectedProject.Path);

                    break;
                case "VisualStudioCode":
                    var vscPath = ConfigManager.LoadValue(ConfigKey.VisualStudioCodePath, string.Empty);
                    if (!File.Exists(vscPath))
                    {
                        await ShowMessageAsync("Error", "The path of Visual Studio Code is not set.");
                        return;
                    }

                    Helper.OpenIn(vscPath, SelectedProject.Folder);
                    break;
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex);
        }
    }

    /// <summary>
    /// Cleans the bin / obj folder of the selected project
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task CleanFolder()
    {
        if (SelectedProject == null)
            return;

        if (await ShowQuestionAsync("Clean",
                "Do you really want to delete the 'bin' and 'obj' folders? This action cannot be undone.", "Yes",
                "No") != MessageDialogResult.Affirmative)
            return;

        var result = Helper.Clean(SelectedProject);

        var header = result ? "Clean - Successful" : "Clean - Error";
        var message = result
            ? "'bin' and 'obj' folder successfully 'cleaned'."
            : "An error has occurred while 'cleaning' the 'bin' and 'obj' folder.";

        await ShowMessageAsync(header, message);
    }

    /// <summary>
    /// Cleans the bin / obj folder of the selected project
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task GitCleanFolder()
    {
        if (SelectedProject == null)
            return;

        if (await ShowQuestionAsync("Clean",
                "Do you really want to delete all 'untracked' files? This action cannot be undone.", "Yes",
                "No") != MessageDialogResult.Affirmative)
            return;

        var result = GitHelper.Clean(SelectedProject);

        var header = result ? "Clean - Successful" : "Clean - Error";
        var message = result
            ? "'bin' and 'obj' folder successfully 'cleaned'."
            : "An error has occurred while 'cleaning' the 'bin' and 'obj' folder.";

        await ShowMessageAsync(header, message);
    }

    /// <summary>
    /// Opens the selected project in the windows explorer
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task RevealInExplorer()
    {
        if (SelectedProject == null)
            return;

        try
        {
            Helper.OpenInExplorer(SelectedProject.Folder);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex);
        }
    }

    /// <summary>
    /// Opens the file info window
    /// </summary>
    [RelayCommand]
    private void ShowFileInfos()
    {
        if (SelectedProject == null)
            return;

        var infoWindow = new FileStatusWindow(SelectedProject)
        {
            Owner = GetMainWindow()
        };

        infoWindow.ShowDialog();
    }

    /// <summary>
    /// Opens the branch list window
    /// </summary>
    [RelayCommand]
    private void ShowBranchList()
    {
        if (SelectedProject == null)
            return;

        var branchWindow = new BranchListWindow(SelectedProject)
        {
            Owner = GetMainWindow()
        };

        branchWindow.ShowDialog();
    }

    /// <summary>
    /// Exports the current list as CSV file
    /// </summary>
    [RelayCommand]
    private async Task ExportAsync()
    {
        if (!Projects.Any())
            return;

        try
        {
            await ExportAsCsvAsync(Projects);
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
        if (!Projects.Any())
            return;

        CopyToClipboard(Projects);
    }

    #endregion
}