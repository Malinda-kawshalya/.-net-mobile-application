using System.Globalization;

namespace Recipes_app.Converters
{
    /// <summary>
    /// Converts a boolean favorite state to a heart icon string.
    /// true → "❤️" (filled), false → "🤍" (outline)
    /// </summary>
    public class FavoriteIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isFavorite)
                return isFavorite ? "❤️" : "🤍";
            return "🤍";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var icon = value?.ToString();
            return icon == "❤️";
        }
    }
}
