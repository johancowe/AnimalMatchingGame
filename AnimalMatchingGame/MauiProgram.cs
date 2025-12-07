using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace AnimalMatchingGame
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

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windows => windows.OnWindowCreated(window =>
                {
                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                    // 4x4 grid: 4*100 + 3*5 spacing + 60 padding + window borders
                    int width = 700;
                    int height = 1080;
                    
                    // Forceer de grootte door eerst te resizen
                    appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
                    
                    // En ook via de presenter
                    if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                    {
                        presenter.IsResizable = true;
                        presenter.IsMaximizable = true;
                        presenter.IsMinimizable = true;
                    }
                    
                    // Centreer het window op het scherm
                    var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
                    var centerX = (displayArea.WorkArea.Width - width) / 2;
                    var centerY = (displayArea.WorkArea.Height - height) / 2;
                    appWindow.MoveAndResize(new Windows.Graphics.RectInt32(centerX, centerY, width, height));
                }));
            });
#endif

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
