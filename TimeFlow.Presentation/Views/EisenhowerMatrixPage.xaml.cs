using Custom.MAUI.Components;
using TimeFlow.Domain.Entities;
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
            MyCalendar.DateDeselected += OnDateDeselected;
            MyCalendar.DateRangeSelected += OnDateRangeSelected;
        }

        private void OnDateSelected(object sender, DayTappedEventArgs selectedDate)
        {
            if (BindingContext is EisenhowerMatrixViewModel viewModel)
            {
                viewModel.ClearTaskEditor();
                viewModel.SelectedStartDate = selectedDate.Date;
                viewModel.SelectedEndDate = selectedDate.Date;
                viewModel.IsTaskEditorVisible = true;
            }
        }

        private void OnDateDeselected(object sender, DateTime deselectedDate)
        {
            if (BindingContext is EisenhowerMatrixViewModel viewModel)
            {
                viewModel.ClearTaskEditor();
                viewModel.IsTaskEditorVisible = false;
            }
        }

        private void OnDateRangeSelected(object sender, DateRangeTappedEventArgs dateRange)
        {
            if (BindingContext is EisenhowerMatrixViewModel viewModel)
            {
                viewModel.ClearTaskEditor();
                viewModel.SelectedStartDate = dateRange.StartDate;
                viewModel.SelectedEndDate = dateRange.EndDate;
                viewModel.IsTaskEditorVisible = true;
            }
        }

        private async void OnTaskCompletionChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TaskItem taskItem)
            {
                taskItem.IsCompleted = e.Value;
                if (BindingContext is EisenhowerMatrixViewModel viewModel)
                {
                    await viewModel.UpdateTaskCompletionStatus(taskItem);
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is EisenhowerMatrixViewModel viewModel)
            {
                viewModel.LoadTasksAsync();
            }
        }

    }
}
