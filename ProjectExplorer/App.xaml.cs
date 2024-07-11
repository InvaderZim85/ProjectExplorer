using System.Windows;
using Microsoft.EntityFrameworkCore;
using ProjectExplorer.Data;

namespace ProjectExplorer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Occurs when the app starts
    /// </summary>
    /// <param name="sender">The <see cref="App"/></param>
    /// <param name="e">The startup event arguments</param>
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        var context = new AppDbContext();
        context.Database.Migrate(); // Create the database if needed and migrate the latest migration files
    }
}