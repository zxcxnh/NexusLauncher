using System.Windows.Controls;
using NexusLauncher.ViewModels;

namespace NexusLauncher.Views;

/// <summary>
/// Simple wrapper that injects the HomeViewModel via DI.
/// </summary>
public partial class HomeView : UserControl
{
    public HomeView(HomeViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
