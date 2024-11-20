using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeFlow.Core.Interfaces;
using TimeFlow.Core.Services;
using TimeFlow.Domain.Entities;
using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITimeBlockService _timeBlockService;

        public string TodayDate => DateTime.Now.ToString("dd MMMM yyyy");

        private ObservableCollection<TimeBlock> _todayTimeBlocks;
        public ObservableCollection<TimeBlock> TodayTimeBlocks
        {
            get => _todayTimeBlocks;
            set => SetProperty(ref _todayTimeBlocks, value);
        }

        public ICommand NavigateToEisenhowerMatrixCommand { get; }
        public ICommand NavigateToTimeBlockingCommand { get; }
        public ICommand StartWorkCommand { get; }

        public MainViewModel(ITimeBlockService taskService)
        {
            _timeBlockService = taskService;
            _todayTimeBlocks = new ObservableCollection<TimeBlock>();

            NavigateToEisenhowerMatrixCommand = new Command(async () => await OnNavigateToEisenhowerMatrix());
            NavigateToTimeBlockingCommand = new Command(async () => await OnNavigateToTimeBlocking());
            StartWorkCommand = new Command(async () => await OnStartWork());
        }

        public async void LoadTimeBlocks()
        {
            var timeBlocks = await _timeBlockService.GetTimeBlocksForTodayAsync();
            TodayTimeBlocks.Clear();
            foreach (var block in timeBlocks)
            {
                TodayTimeBlocks.Add(block);
            }
        }

        private async Task OnNavigateToEisenhowerMatrix()
        {
            await Shell.Current.GoToAsync("//EisenhowerMatrixPage");
        }

        private async Task OnNavigateToTimeBlocking()
        {
            await Shell.Current.GoToAsync("//TimeBlockingPage");
        }

        private async Task OnStartWork()
        {
            await Shell.Current.GoToAsync("//PomodoroPage");
        }
    }
}
