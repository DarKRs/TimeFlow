﻿using System.Windows.Input;

namespace TimeFlow.Presentation.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        public string TodayDate => DateTime.Now.ToString("dd MMMM yyyy");

        public ICommand NavigateToEisenhowerMatrixCommand { get; }
        public ICommand NavigateToTimeBlockingCommand { get; }
        public ICommand StartWorkCommand { get; }

        public MainViewModel()
        {
            NavigateToEisenhowerMatrixCommand = new Command(async () => await OnNavigateToEisenhowerMatrix());
            StartWorkCommand = new Command(async () => await OnStartWork());
        }

        private async Task OnNavigateToEisenhowerMatrix()
        {
            await Shell.Current.GoToAsync("//EisenhowerMatrixPage");
        }


        private async Task OnStartWork()
        {
            await Shell.Current.GoToAsync("//PomodoroPage");
        }
    }
}
