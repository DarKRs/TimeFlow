using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    [QueryProperty(nameof(ScheduledDate), "ScheduledDate")]
    public partial class AddTaskPage : ContentPage
    {
        private DateTime _scheduledDate;
        public DateTime ScheduledDate
        {
            get => _scheduledDate;
            set
            {
                _scheduledDate = value;
                if (BindingContext is AddTaskViewModel viewModel)
                {
                    viewModel.ScheduledDate = _scheduledDate;
                }
            }
        }

        public AddTaskPage(AddTaskViewModel addTaskViewModel)
        {
            InitializeComponent();
            BindingContext = addTaskViewModel;
        }
    }
}
