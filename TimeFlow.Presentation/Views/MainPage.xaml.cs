﻿using TimeFlow.Presentation.ViewModels;

namespace TimeFlow.Presentation.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

    }

}
