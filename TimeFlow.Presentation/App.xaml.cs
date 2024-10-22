using Plugin.LocalNotification.EventArgs;
using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Устанавливаем MainPage после инициализации ресурсов
            MainPage = _serviceProvider.GetRequiredService<MainPage>();

            MainPage = new AppShell();
        }

        //Обработка уведомлений
        private void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            if (e.IsDismissed)
            {
                // Действие при закрытии уведомления
                return;
            }

            if (e.IsTapped)
            {
                // Действие при нажатии на уведомление
                return;
            }
        }
    }
}
