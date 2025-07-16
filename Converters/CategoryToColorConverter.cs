using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FlowerGame.Converters
{
    public class CategoryToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var category = value as string;
            var selectedCategory = parameter as string;

            if (!string.IsNullOrEmpty(category) && category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase))
                return Colors.Purple; // Highlight color

            return Colors.LightGray; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}