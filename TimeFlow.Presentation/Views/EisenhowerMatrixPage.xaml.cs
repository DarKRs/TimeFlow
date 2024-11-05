using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class EisenhowerMatrixPage : ContentPage
    {
        public EisenhowerMatrixPage(EisenhowerMatrixViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            MyCalendar.DateSelected += OnDateSelected;
        }

        private async void OnDateSelected(object sender, DateTime selectedDate)
        {
            string action = await DisplayActionSheet($"Вы выбрали {selectedDate:dd MMMM yyyy}", "Отмена", null, "Просмотреть задачи", "Создать задачу");

            if (action == "Просмотреть задачи")
            {
                // Отобразить задачи на выбранную дату
                if (BindingContext is EisenhowerMatrixViewModel viewModel)
                {
                   // viewModel.LoadTasksForDate(selectedDate);
                }
            }
            else if (action == "Создать задачу")
            {
                await Shell.Current.GoToAsync($"{nameof(AddTaskPage)}?ScheduledDate={selectedDate:yyyy-MM-dd}");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is EisenhowerMatrixViewModel viewModel)
            {
                viewModel.LoadTasks();
            }
        }
    }
}
