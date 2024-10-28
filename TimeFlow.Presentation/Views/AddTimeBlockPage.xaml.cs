using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class AddTimeBlockPage : ContentPage
    {
        public AddTimeBlockPage(AddTimeBlockViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AddTimeBlockViewModel viewModel)
            {
                viewModel.LoadTasks();
            }
        }
    }
}
