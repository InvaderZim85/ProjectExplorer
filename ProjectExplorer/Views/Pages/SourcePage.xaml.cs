using ProjectExplorer.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ProjectExplorer.Views.Pages;

/// <summary>
/// Interaction logic for SourcePage.xaml
/// </summary>
public partial class SourcePage : INavigableView<SourceViewModel>
{
    /// <inheritdoc />
    public SourceViewModel ViewModel { get; }

    public SourcePage(SourceViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}