using System.Collections.ObjectModel;

namespace ProjectExplorer.Common;

/// <summary>
/// Provides several helper methods
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Converts the <paramref name="source"/> into an observable collection which is needed for the WPF ui (MVVM)
    /// </summary>
    /// <typeparam name="T">The type of the entries</typeparam>
    /// <param name="source">The original list</param>
    /// <returns>The observable collection</returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }
}