
namespace RedditRoulette.Converters
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double fontSize)
            {
                double screenWidth = App.Current.MainPage.Width; // Bildschirmbreite
                double screenHeight = App.Current.MainPage.Height; // Bildschirmhöhe

                // Hier kannst du deine Logik für die Schriftgrößenanpassung basierend auf der Bildschirmgröße implementieren
                // Zum Beispiel könntest du die Schriftgröße proportional zur Bildschirmgröße skalieren

                double scaledFontSize = CalculateScaledFontSize(fontSize, screenWidth, screenHeight);

                return scaledFontSize;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private double CalculateScaledFontSize(double fontSize, double screenWidth, double screenHeight)
        {
            // Hier kannst du die Logik implementieren, um die Schriftgröße basierend auf der Bildschirmgröße anzupassen
            // Zum Beispiel könntest du die Schriftgröße proportional zur Bildschirmgröße skalieren
            double scaleFactor = (screenWidth + screenHeight) / 1000; // Beispiel-Skalierungsfaktor

            double scaledFontSize = fontSize * scaleFactor;

            return scaledFontSize;
        }
    }
}
