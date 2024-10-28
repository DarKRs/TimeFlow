using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    [QueryProperty(nameof(TimeBlock), "TimeBlock")]
    public partial class EditTimeBlockPage : ContentPage
    {
        private EditTimeBlockViewModel ViewModel => BindingContext as EditTimeBlockViewModel;
        public EditTimeBlockPage(EditTimeBlockViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private TimeBlock _timeBlock;
        public TimeBlock TimeBlock
        {
            get => _timeBlock;
            set
            {
                _timeBlock = value;
                if (ViewModel != null && _timeBlock != null)
                {
                    ViewModel.Initialize(_timeBlock);
                }
            }
        }
    }
}
