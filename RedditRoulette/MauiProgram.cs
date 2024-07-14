using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedditRoulette.Services;
using RedditRoulette.ViewModel;
using RedditRoulette.Model;
using System.Reflection;

namespace RedditRoulette
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Konfiguration hinzufügen
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("RedditRoulette.appsettings.json");
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);

            // Korrigierte Zeile
            builder.Services.Configure<AppConfig>(options =>
                builder.Configuration.GetSection("AppConfig").Bind(options));

            builder.Services.AddDataProtection();
            builder.Services.AddSingleton<EncryptionService>();
            builder.Services.AddSingleton<RedditApiService>();
            builder.Services.AddSingleton<FileService>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<App>();

            return builder.Build();
        }
    }
}