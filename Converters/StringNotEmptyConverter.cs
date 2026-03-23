using System.Globalization;

namespace Recipes_app.Converters
{
    /// <summary>
    /// Converts a string to boolean (true if not null or empty).
    /// Useful for IsVisible bindings on optional fields.
    /// </summary>
    public class StringNotEmptyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value?.ToString());
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool hasText)
                return hasText ? "value" : string.Empty;

            return string.Empty;
        }
    }
}
