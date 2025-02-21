using Avalonia;

namespace PerpetuaNet;

public class Program
{
    // Main é o ponto de entrada do aplicativo
    public static void Main(string[] args)
    {
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
    }
}