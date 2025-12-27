// ════════════════════════════════════════════════════════════════════
// ValueConverters.cs - Value converters for XAML bindings
// ════════════════════════════════════════════════════════════════════

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RustControlPanel.Converters
{
    /// <summary>
    /// Converts health value to width for health bar.
    /// </summary>
    public class HealthToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float health)
            {
                // Health is 0-100, convert to width (max 50px)
                return Math.Clamp(health / 100.0 * 50.0, 0, 50);
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts health value to color (red → yellow → green).
    /// </summary>
    public class HealthToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is float health)
            {
                if (health > 60)
                    return Color.FromRgb(16, 185, 129); // Green
                else if (health > 30)
                    return Color.FromRgb(251, 191, 36); // Yellow
                else
                    return Color.FromRgb(239, 68, 68); // Red
            }
            return Colors.Gray;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
