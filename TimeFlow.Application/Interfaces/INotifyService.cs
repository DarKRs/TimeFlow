namespace TimeFlow.Core.Interfaces
{
    public interface INotifyService
    {
        Task ShowNotificationAsync(string title, string message);
    }
}
