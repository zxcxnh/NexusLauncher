using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using NexusLauncher.Views;

namespace NexusLauncher.Infrastructure;

/// <summary>
/// Very small navigation helper – swaps a ContentControl's Content.
/// </summary>
public sealed class NavigationService
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<Type, UserControl> _cache = new();

    public NavigationService(IServiceProvider provider) => _provider = provider;

    public void Navigate(ContentControl host, Type viewType)
    {
        if (!_cache.TryGetValue(viewType, out var view))
        {
            view = (UserControl)_provider.GetRequiredService(viewType);
            _cache[viewType] = view;
        }

        host.Content = view;
    }

    // Convenience overloads
    public void NavigateToHome(ContentControl host) => Navigate(host, typeof(HomeView));
    public void NavigateToSettings(ContentControl host) => Navigate(host, typeof(SettingsView));
}
