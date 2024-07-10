using Microsoft.Win32;
using ProjectExplorer.Business;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ProjectExplorer.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    /// <summary>
    /// Contains the value which indicates whether the settings were loaded
    /// </summary>
    private bool _isInitialized;

    /// <summary>
    /// Gets or sets the app version
    /// </summary>
    [ObservableProperty]
    private string _appVersion = string.Empty;

    /// <summary>
    /// Gets or sets the current theme
    /// </summary>
    [ObservableProperty]
    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

    /// <summary>
    /// Gets or sets the path of the visual studio executable
    /// </summary>
    [ObservableProperty]
    private string _visualStudioPath = string.Empty;

    /// <inheritdoc />
    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    /// <inheritdoc />
    public void OnNavigatedFrom()
    {
        // Save the settings
        SettingsManager.Settings.VisualStudioPath = VisualStudioPath;

        SettingsManager.SaveSettings();
    }

    private void InitializeViewModel()
    {
        CurrentTheme = ApplicationThemeManager.GetAppTheme();
        AppVersion = $"ProjectExplorer - {GetAssemblyVersion()}";

        VisualStudioPath = SettingsManager.Settings.VisualStudioPath;

        _isInitialized = true;
    }

    /// <summary>
    /// Gets the version of this program
    /// </summary>
    /// <returns>The assembly version</returns>
    private string GetAssemblyVersion()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
               ?? string.Empty;
    }

    /// <summary>
    /// Occurs when the theme was changed
    /// </summary>
    /// <param name="parameter">The theme parameter</param>
    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentTheme == ApplicationTheme.Light)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Light);
                CurrentTheme = ApplicationTheme.Light;

                break;

            default:
                if (CurrentTheme == ApplicationTheme.Dark)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                CurrentTheme = ApplicationTheme.Dark;

                break;
        }
    }

    /// <summary>
    /// Occurs when the browse button gets clicked
    /// </summary>
    [RelayCommand]
    private void BrowseForExe()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Executable (*.exe)|*.exe"
        };

        if (dialog.ShowDialog() != true)
            return;

        VisualStudioPath = dialog.FileName;
    }
}