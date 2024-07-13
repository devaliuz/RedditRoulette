

namespace RedditRoulette.Converters
{
    public class HeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double screenHeight = (double)value;
            double desiredHeight = 0;

            if (IsDesktopPlatform())
            {
                // Nur für Desktop-Geräte die WindowHeight verwenden
                double windowHeight = App.Current.MainPage.Height;
                desiredHeight = windowHeight * 0.68;
            }
            else
            {
                // Für Mobilgeräte die ScreenHeight verwenden
                desiredHeight = screenHeight * 0.68;
            }

            return desiredHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool IsDesktopPlatform()
        {
            //TODO!!!
            // Hier implementierst du die Logik, um die Desktop-Plattform zu erkennen
            // Zum Beispiel könntest du DependencyService verwenden, um die Plattform zu identifizieren
            // Rückgabe true für Desktop, false für Mobilgeräte
            return false;
        }
    }
}
