// Authors: Conner Fisk, Ethan Bowles
// Class that represents a maui aplication
// Date: Nov 15, 2022

namespace SnakeGame;

public static class MauiProgram
{
    /// <summary>
    /// Maui app setup
    /// </summary>
    /// <returns></returns>
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

        return builder.Build();
    }
}

