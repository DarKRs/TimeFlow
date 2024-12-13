using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.Views;
using TimeFlow.Presentation.Views.Popups;

namespace TimeFlow.Presentation.ViewModels.Popups
{
    public class AddHabitPopupViewModel : BaseViewModel
    {
        private readonly IHabitService _habitService;

        public string HabitName { get; set; }
        public string HabitDescription { get; set; }

        public ICommand SaveHabitCommand { get; }
        public ICommand ClosePopupCommand { get; }

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
                // Отобразить ошибку
                return;
            }

            var newHabit = new Habit
            {
                Name = HabitName,
                Description = HabitDescription,
                CreatedDate = DateTime.UtcNow
            };

            await _habitService.CreateHabitAsync(newHabit);
            ClosePopup();
        }

        private void ClosePopup()
        {
            (Application.Current.MainPage as IView)?.Handler?.MauiContext ?.Services.GetService<AddHabitPopup>().Close();
        }
    }
}
