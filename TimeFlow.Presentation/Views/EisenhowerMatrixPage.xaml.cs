using Custom.MAUI.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
