using Microsoft.Extensions.Logging;
using FlowerGame.Services;
using FlowerGame.ViewModel;

namespace FlowerGame
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

            // Register Services in correct order
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<TranslationService>();
            builder.Services.AddSingleton<WordService>();
            builder.Services.AddSingleton<GameService>();
            

            // Register ViewModels
            builder.Services.AddTransient<WordsViewModel>();
            builder.Services.AddTransient<GameViewModel>();

            // Register Views
            builder.Services.AddTransient<Views.MainPage>();
            builder.Services.AddTransient<Views.GamePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}