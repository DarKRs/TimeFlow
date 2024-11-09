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
        private readonly ITaskService _taskService;

        public string TodayDate => DateTime.Now.ToString("dd MMMM yyyy");

        private ObservableCollection<TaskItem> _todayTasks;
        public ObservableCollection<TaskItem> TodayTasks
        {
            get => _todayTasks;
            set => SetProperty(ref _todayTasks, value);
        }

        public ICommand NavigateToEisenhowerMatrixCommand { get; }
        public ICommand NavigateToTimeBlockingCommand { get; }
        public ICommand StartWorkCommand { get; }

        public MainViewModel(ITaskService taskService)
        {
            _taskService = taskService;
            _todayTasks = new ObservableCollection<TaskItem>();

            NavigateToEisenhowerMatrixCommand = new Command(async () => await OnNavigateToEisenhowerMatrix());
            NavigateToTimeBlockingCommand = new Command(async () => await OnNavigateToTimeBlocking());
            StartWorkCommand = new Command(async () => await OnStartWork());
        }

        public async void LoadTasks()
        {
            var tasks = await _taskService.GetTasksForTodayAsync();
            _todayTasks.Clear();
            foreach (var task in tasks)
            {
                _todayTasks.Add(task);
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
