using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using Microsoft.Win32;
using ProjectExplorer.Business;
using ProjectExplorer.Common.Enums;
using ProjectExplorer.Models.Internal;
using ProjectExplorer.Ui.View;
using System.Collections.ObjectModel;
using System.IO;

namespace ProjectExplorer.Ui.ViewModel;

/// <summary>
/// Interaction logic for <see cref="View.SettingsControl"/>
/// </summary>
internal partial class SettingsControlViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the path of visual studio
    /// </summary>
    [ObservableProperty]
    private string _visualStudioPath = string.Empty;

    /// <summary>
    /// Occurs when the value of <see cref="VisualStudioPath"/> was changed
    /// </summary>
    /// <param name="value">The new value</param>
    partial void OnVisualStudioPathChanged(string value)
    {
        OpenInVisualStudioEnabled = File.Exists(value);
    }

    /// <summary>
    /// Gets or sets the value which indicates whether the "open in visual studio" option is enabled
    /// </summary>
    [ObservableProperty]
    private bool _openInVisualStudioEnabled;

    /// <summary>
    /// Gets or sets the path of Visual Studio Code.
    /// </summary>
    [ObservableProperty]
    private string _visualStudioCodePath = string.Empty;

    /// <summary>
    /// Occurs when the value of <see cref="VisualStudioCodePath"/> was changed
    /// </summary>
    /// <param name="value">The new value</param>
    partial void OnVisualStudioCodePathChanged(string value)
    {
        OpenInVisualStudioCodeEnabled = File.Exists(value);
    }

    /// <summary>
    /// Gets or sets the value which indicates whether the "open in visual studio code" option is enabled
    /// </summary>
    [ObservableProperty]
    private bool _openInVisualStudioCodeEnabled;

    /// <summary>
    /// The list with the color themes
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ColorEntry> _colorThemeList = [];

    /// <summary>
    /// Backing field for <see cref="SelectedColorTheme"/>
    /// </summary>
    private ColorEntry? _selectedColorTheme;

    /// <summary>
    /// Gets or sets the selected color theme
    /// </summary>
    public ColorEntry? SelectedColorTheme
    {
        get => _selectedColorTheme;
        set
        {
            if (SetProperty(ref _selectedColorTheme, value) && value != null)
                ThemeHelper.SetColorTheme(value.Name);
        }
    }

    /// <summary>
    /// Init the view model
    /// </summary>
    public void InitViewModel()
    {
        VisualStudioPath = ConfigManager.LoadValue(ConfigKey.VisualStudioPath, string.Empty);
        VisualStudioCodePath = ConfigManager.LoadValue(ConfigKey.VisualStudioCodePath, string.Empty);
        var theme = ConfigManager.LoadValue(ConfigKey.ColorTheme, "Cyan");

        AddColors(theme);
    }

    /// <summary>
    /// Adds the window to add a new color
    /// </summary>
    [RelayCommand]
    private void AddCustomColor()
    {
        var dialog = new CustomColorWindow
        {
            Owner = GetMainWindow()
        };

        if (dialog.ShowDialog() != true)
            return;

        AddColors(dialog.ColorName);
    }

    /// <summary>
    /// Deletes the selected custom color
    /// </summary>
    [RelayCommand]
    private async Task DeleteCustomColorAsync()
    {
        if (SelectedColorTheme == null)
            return;

        if (!SelectedColorTheme.CustomColor)
        {
            await ShowMessageAsync("Color", "You can't delete a default color. Please select a custom color.");
            return;
        }

        // Save the custom colors
        var result = ThemeHelper.RemoveCustomColor(SelectedColorTheme.Name);

        if (result)
        {
            AddColors(ColorThemeList.FirstOrDefault()?.Name ?? string.Empty);
        }
        else
        {
            await ShowMessageAsync("Color",
                $"An error has occurred while deleting the color '{SelectedColorTheme.Name}'");
        }
    }

    /// <summary>
    /// Saves the current theme
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        if (SelectedColorTheme == null)
            return;

        var controller = await ShowProgressAsync("Save", "Please wait while saving the settings...");

        try
        {
            // Save the settings
            await ConfigManager.SaveValuesAsync(new SortedList<ConfigKey, object>
            {
                { ConfigKey.VisualStudioPath, VisualStudioPath },
                { ConfigKey.VisualStudioCodePath, VisualStudioCodePath },
                { ConfigKey.ColorTheme, SelectedColorTheme }
            });
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Browses for an application
    /// </summary>
    /// <param name="type">The desired type</param>
    [RelayCommand]
    private void BrowseApplication(string type)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Executable (*.exe)|*.exe"
        };

        if (dialog.ShowDialog() != true)
            return;

        switch (type)
        {
            case "VisualStudio":
                VisualStudioPath = dialog.FileName;
                break;
            case "VisualStudioCode":
                VisualStudioCodePath = dialog.FileName;
                break;
        }
    }

    /// <summary>
    /// Adds the colors to the <see cref="ColorThemeList"/> and sets the <see cref="SelectedColorTheme"/>.
    /// </summary>
    /// <param name="preSelection">The name of the color which should be selected</param>
    private void AddColors(string preSelection)
    {
        var tmpList = new List<ColorEntry>();
        tmpList.AddRange(ThemeManager.Current.ColorSchemes.Select(s => new ColorEntry(s, false)));

        // Add the custom colors
        tmpList.AddRange(ThemeHelper.LoadCustomColors().Select(s => s.Name).Select(s => new ColorEntry(s, true)));

        ColorThemeList = new ObservableCollection<ColorEntry>(tmpList);

        SelectedColorTheme = ColorThemeList.FirstOrDefault(f => f.Name.Equals(preSelection, StringComparison.OrdinalIgnoreCase));
    }
}