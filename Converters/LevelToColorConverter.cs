using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FlowerGame.Converters
{
    public class LevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = value as string;
            var selectedLevel = parameter as string;

            if (!string.IsNullOrEmpty(level) && level.Equals(selectedLevel, StringComparison.OrdinalIgnoreCase))
                return Colors.Orange; // Highlight color

            return Colors.LightGray; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}