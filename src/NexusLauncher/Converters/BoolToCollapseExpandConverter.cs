using System;
using System.Globalization;
using System.Windows.Data;

namespace NexusLauncher.Converters;

/// <summary>
/// Returns "«" when true (collapsed) and "»" when false (expanded).
/// </summary>
public sealed class BoolToCollapseExpandConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? "«" : "»";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
