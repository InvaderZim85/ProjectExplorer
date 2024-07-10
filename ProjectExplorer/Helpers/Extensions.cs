using System.Collections.ObjectModel;

namespace ProjectExplorer.Helpers;

/// <summary>
/// Provides several helper functions
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Converts the given "list" into an observable collection
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    /// <param name="source">The source list</param>
    /// <returns>The observable collection</returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }
}