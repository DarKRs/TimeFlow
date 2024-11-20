
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class PomodoroPage : ContentPage
    {
        public PomodoroPage(PomodoroViewModel pomodoroViewModel)
        {
            InitializeComponent();
            BindingContext = pomodoroViewModel;
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
