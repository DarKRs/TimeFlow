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
