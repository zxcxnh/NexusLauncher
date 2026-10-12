using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NexusLauncher.Infrastructure;

/// <summary>
/// Converts a bool to Visibility (true => Visible, false => Collapsed)
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
