using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using Serilog;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using ZimLabs.TableCreator;
using Timer = System.Timers.Timer;

namespace ProjectExplorer.Ui.ViewModel;

/// <summary>
/// Provides the base functions of a view model
/// </summary>
internal partial class ViewModelBase : ObservableObject
{
    /// <summary>
    /// The different error types
    /// </summary>
    public enum ErrorMessageType
    {
        /// <summary>
        /// General error
        /// </summary>
        General,

        /// <summary>
        /// Error while saving
        /// </summary>
        Load,

        /// <summary>
        /// Error while loading
        /// </summary>
        Save,

        /// <summary>
        /// Error while deleting
        /// </summary>
        Delete
    }

    /// <summary>
    /// The instance of the mah apps dialog coordinator
    /// </summary>
    private readonly IDialogCoordinator _dialogCoordinator;

    /// <summary>
    /// The message timer
    /// </summary>
    private readonly Timer _messageTimer = new(TimeSpan.FromSeconds(10).TotalMilliseconds);

    /// <summary>
    /// Gets or sets the info message
    /// </summary>
    [ObservableProperty]
    private string _infoMessage = string.Empty;

    /// <summary>
    /// Creates a new instance of the <see cref="ViewModelBase"/>
    /// </summary>
    protected ViewModelBase()
    {
        _dialogCoordinator = DialogCoordinator.Instance;
    }

    /// <summary>
    /// Shows a message dialog
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message of the dialog</param>
    /// <returns>The awaitable task</returns>
    protected async Task ShowMessageAsync(string title, string message)
    {
        await _dialogCoordinator.ShowMessageAsync(this, title, message);
    }

    /// <summary>
    /// Shows a question dialog with two buttons
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message of the dialog</param>
    /// <param name="okButtonText">The text of the ok button (optional)</param>
    /// <param name="cancelButtonText">The text of the cancel button (optional)</param>
    /// <returns>The dialog result</returns>
    protected async Task<MessageDialogResult> ShowQuestionAsync(string title, string message, string okButtonText = "OK",
        string cancelButtonText = "Cancel")
    {
        var result = await _dialogCoordinator.ShowMessageAsync(this, title, message,
            MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AffirmativeButtonText = okButtonText,
                NegativeButtonText = cancelButtonText
            });

        return result;
    }

    /// <summary>
    /// Shows an error message and logs the exception
    /// </summary>
    /// <param name="ex">The exception which was thrown</param>
    /// <param name="messageType">The desired message type</param>
    /// <param name="caller">The name of the method, which calls this method. Value will be filled automatically</param>
    /// <returns>The awaitable task</returns>
    protected async Task ShowErrorAsync(Exception ex, ErrorMessageType messageType = ErrorMessageType.General,
        [CallerMemberName] string caller = "")
    {
        LogError(ex, caller);

        var message = messageType switch
        {
            ErrorMessageType.Load => "An error has occurred while loading the data.",
            ErrorMessageType.Save => "An error has occurred while saving the data.",
            ErrorMessageType.Delete => "An error has occurred while deleting the data.",
            _ => "An error has occurred."
        };

        await _dialogCoordinator.ShowMessageAsync(this, "Error", message);
    }

    /// <summary>
    /// Logs an error
    /// </summary>
    /// <param name="ex">The exception which was thrown</param>
    /// <param name="caller">The name of the method, which calls this method. Value will be filled automatically</param>
    protected static void LogError(Exception ex, [CallerMemberName] string caller = "")
    {
        // Here your logging function like the following (needs Serilog)
        Log.Error(ex, "An error has occurred. Caller: {caller}", caller);
    }

    /// <summary>
    /// Shows a progress dialog
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message of the dialog</param>
    /// <param name="ctSource">The cancellation token source (optional)</param>
    /// <returns>The dialog controller</returns>
    protected async Task<ProgressDialogController> ShowProgressAsync(string title, string message, CancellationTokenSource? ctSource = default)
    {
        var controller = await _dialogCoordinator.ShowProgressAsync(this, title, message, ctSource != null);
        controller.SetIndeterminate();

        if (ctSource != null)
        {
            controller.Canceled += (_, _) => ctSource.Cancel();
        }

        return controller;
    }

    /// <summary>
    /// Copies the content to the clipboard
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    /// <param name="source">The source</param>
    protected static void CopyToClipboard<T>(IEnumerable<T> source) where T : class
    {
        var content = source.CreateTable(OutputType.Csv);
        Clipboard.SetText(content);
    }

    /// <summary>
    /// Gets the main window
    /// </summary>
    /// <returns>The main window</returns>
    protected Window? GetMainWindow()
    {
        return Application.Current.MainWindow;
    }

    /// <summary>
    /// Shows an info message for 10 seconds
    /// </summary>
    /// <param name="message">The message which should be shown</param>
    protected void ShowInfoMessage(string message)
    {
        InfoMessage = message;
        _messageTimer.Start();
    }

    /// <summary>
    /// Exports the content as CSV file
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    /// <param name="source">The source</param>
    /// <returns></returns>
    protected Task ExportAsCsvAsync<T>(IEnumerable<T> source) where T : class
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CSV file (*.csv)|*.csv"
        };

        return dialog.ShowDialog() != true 
            ? Task.CompletedTask 
            : source.SaveTableAsync(dialog.FileName, OutputType.Csv);
    }
}
