using Plugin.LocalNotification;
using TimeFlow.Core.Interfaces;

namespace TimeFlow.Presentation.Services
{
    internal class AndroidNotificationService : INotifyService
    {
        public async Task ShowNotificationAsync(string title, string message)
        {
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = title,
                Description = message,
                ReturningData = "PomodoroFinished",
                Schedule = { NotifyTime = DateTime.Now }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }
    }
}
