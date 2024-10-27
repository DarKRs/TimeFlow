using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class AddTaskPage : ContentPage
    {
        public AddTaskPage(AddTaskViewModel addTaskViewModel)
        {
            InitializeComponent();
            BindingContext = addTaskViewModel;
        }
    }
}
