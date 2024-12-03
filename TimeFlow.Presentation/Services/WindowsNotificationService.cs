#if WINDOWS
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

using TimeFlow.Core.Interfaces;

namespace TimeFlow.Presentation.Services
{
    internal class WindowsNotificationService : INotifyService
    {
        public Task ShowNotificationAsync(string title, string message)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(message));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("PomodoroApp").Show(toast);

            return Task.CompletedTask;
        }
    }
}
#endif