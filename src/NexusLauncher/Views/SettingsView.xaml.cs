using System.Windows.Controls;
using NexusLauncher.ViewModels;

namespace NexusLauncher.Views;

/// <summary>
/// Settings view – injected via DI.
/// </summary>
public partial class SettingsView : UserControl
{
    public SettingsView(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
