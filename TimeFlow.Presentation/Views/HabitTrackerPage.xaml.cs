using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class HabitTrackerPage : ContentPage
    {
        public HabitTrackerPage(HabitTrackerViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
