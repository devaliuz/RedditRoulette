
using System.Globalization;


namespace RedditRoulette.Converters
{
    public class TextTruncationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                int maxLength = 40; // Maximale Länge des abgekürzten Texts
                if (text.Length > maxLength)
                {
                    return text.Substring(0, maxLength) + "...";
                }
                else
                {
                    return text;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

