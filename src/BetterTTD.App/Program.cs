using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;

namespace BetterTTD.App
{
    internal static class Program
    {
        public static void Main(string[] args) => 
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

        private static AppBuilder BuildAvaloniaApp() => 
            AppBuilder
                .Configure<App>()
                .UseReactiveUI()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
