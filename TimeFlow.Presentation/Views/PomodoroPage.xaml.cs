
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class PomodoroPage : ContentPage
    {
        public PomodoroPage(PomodoroViewModel pomodoroViewModel)
        {
            InitializeComponent();
            BindingContext = pomodoroViewModel;

            pomodoroViewModel.TabPanel = TabPanel;
        }

        private async void OnTaskCompletionChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TaskItem taskItem)
            {
                taskItem.IsCompleted = e.Value; 
                if (BindingContext is PomodoroViewModel viewModel)
                {
                    await viewModel.UpdateTaskCompletionStatus(taskItem); 
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is PomodoroViewModel viewModel)
            {
                viewModel.LoadTodayTasks();
            }
        }
    }
}
