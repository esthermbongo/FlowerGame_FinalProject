using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FlowerGame.Converters
{
    public class LanguageToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value: the language for this item
            // parameter: the selected language
            var language = value as string;
            var selectedLanguage = parameter as string;

            if (!string.IsNullOrEmpty(language) && language.Equals(selectedLanguage, StringComparison.OrdinalIgnoreCase))
                return Colors.DodgerBlue; // Highlight color

            return Colors.LightGray; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}