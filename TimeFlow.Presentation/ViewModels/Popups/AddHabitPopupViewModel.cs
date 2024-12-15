using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.ViewModels.Popups
{
    public class AddHabitPopupViewModel : BaseViewModel
    {
        private readonly IHabitService _habitService;

        public string HabitName { get; set; }
        public string HabitDescription { get; set; }

        public ICommand SaveHabitCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public event Action<Habit> OnHabitAdded;
        public event Action OnPopupClosed;

        public AddHabitPopupViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            SaveHabitCommand = new Command(async () => await SaveHabitAsync());
            ClosePopupCommand = new Command(ClosePopup);
        }

        private async Task SaveHabitAsync()
        {
            if (string.IsNullOrWhiteSpace(HabitName))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Название привычки не может быть пустым", "ОК");
                return;
            }

            var newHabit = new Habit
            {
                Name = HabitName,
                Description = HabitDescription,
                CreatedDate = DateTime.UtcNow
            };

            await _habitService.CreateHabitAsync(newHabit);

            OnHabitAdded?.Invoke(newHabit);

            ClosePopup();
        }

        private void ClosePopup()
        {
            OnPopupClosed?.Invoke();
        }

    }
}
