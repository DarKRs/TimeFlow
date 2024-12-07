using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.ViewModels
{
    public class HabitTrackerViewModel : BaseViewModel
    {
        private readonly IHabitService _habitService;

        public ObservableCollection<Habit> Habits { get; set; }

        public ICommand UpdateHabitStatusCommand { get; }

        public HabitTrackerViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            Habits = new ObservableCollection<Habit>();

            UpdateHabitStatusCommand = new Command<Habit>(async (habit) => await UpdateHabitStatus(habit));

            LoadHabits();
        }

        private async void LoadHabits()
        {
            var habits = await _habitService.GetAllHabitsAsync();
            Habits.Clear();
            foreach (var habit in habits)
            {
                Habits.Add(habit);
            }
        }

        private async Task UpdateHabitStatus(Habit habit)
        {
            if (habit != null)
            {
                await _habitService.UpdateHabitAsync(habit);
                LoadHabits(); 
            }
        }
    }
}
