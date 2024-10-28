using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class TimeBlockingPage : ContentPage
    {
        public TimeBlockingPage(TimeBlockingViewModel timeBlockingViewModel)
        {
            InitializeComponent();
            BindingContext = timeBlockingViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TimeBlockingViewModel viewModel)
            {
                viewModel.LoadData();
            }
        }
    }
}
