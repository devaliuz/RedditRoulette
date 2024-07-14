using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditRoulette.Converters
{
    public class UrlToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string url)
            {
                if (url.EndsWith(".gif") || url.EndsWith(".jpg") || url.EndsWith(".png"))
                {
                    return ImageSource.FromUri(new Uri(url));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
