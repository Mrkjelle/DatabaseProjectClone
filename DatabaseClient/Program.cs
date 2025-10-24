using System;
using Avalonia;

namespace DatabaseClient
{
    internal class Program
    {
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("crash.txt", ex.ToString());
                throw;
            }
        }
    }
}