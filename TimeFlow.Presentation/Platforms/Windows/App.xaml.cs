using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimeFlow.Presentation.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            // Получаем текущее окно приложения
            var mauiWindow = Application.Windows.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

            if (mauiWindow != null)
            {
                var appWindow = GetAppWindowForCurrentWindow(mauiWindow);
                if (appWindow != null)
                {
                    // Создаём презентер для окна
                    var presenter = appWindow.Presenter as OverlappedPresenter;
                    if (presenter != null)
                    {
                        // Разрешаем изменение размера и убираем режим полного экрана
                        presenter.IsResizable = true;
                        presenter.IsMaximizable = true;
                        presenter.SetBorderAndTitleBar(true, true);
                    }

                    // Устанавливаем размеры окна на весь экран
                    var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);
                    var workArea = displayArea.WorkArea;
                    appWindow.MoveAndResize(new RectInt32(workArea.X, workArea.Y, workArea.Width, workArea.Height));
                }
            }
        }

        private AppWindow? GetAppWindowForCurrentWindow(Microsoft.UI.Xaml.Window mauiWindow)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(mauiWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(windowId);
        }
    }

}
