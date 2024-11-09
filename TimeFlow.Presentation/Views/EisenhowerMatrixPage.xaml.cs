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

        private void OnDateSelected(object sender, DateTime selectedDate)
        {
            if (BindingContext is EisenhowerMatrixViewModel viewModel)
            {
                viewModel.ShowTaskEditor(selectedDate);
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
