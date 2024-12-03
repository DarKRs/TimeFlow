using System.ComponentModel;
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

            pomodoroViewModel.PropertyChanged += ViewModel_PropertyChanged;
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

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PomodoroViewModel.IsPanelVisible))
            {
                if (BindingContext is PomodoroViewModel viewModel)
                {
                    if (viewModel.IsPanelVisible)
                    {
                        TabPanel.TranslationX = 420;
                        TabPanel.IsVisible = true;
                        await TabPanel.TranslateTo(0, 0, 350, Easing.SinInOut);
                    }
                    else
                    {
                        await TabPanel.TranslateTo(420, 0, 350, Easing.SinInOut);
                        TabPanel.IsVisible = false;
                    }
                }
            }
        }
    }
}
