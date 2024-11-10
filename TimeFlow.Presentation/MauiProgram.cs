﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using System.Diagnostics;
using TimeFlow.Core.Interfaces;
using TimeFlow.Core.Services;
using TimeFlow.Infrastructure.Data;
using TimeFlow.Infrastructure.Repositories;
using TimeFlow.Presentation.ViewModels;
using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                }
                );

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "timeflow.db");
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                Debug.WriteLine("Настройка DbContext");
                options.UseSqlite($"Data Source={dbPath}");
            });

            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<IPomodoroSessionRepository, PomodoroSessionRepository>();
            builder.Services.AddScoped<ITimeBlockRepository, TimeBlockRepository>();
            builder.Services.AddScoped<ITaskService,TaskService>();
            builder.Services.AddScoped<ITimeBlockService, TimeBlockService>();

            // Регистрация ViewModel и страниц
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<PomodoroViewModel>();
            builder.Services.AddTransient<EisenhowerMatrixViewModel>();
            builder.Services.AddTransient<TimeBlockingViewModel>();
            builder.Services.AddTransient<AddTimeBlockViewModel>();
            builder.Services.AddTransient<EditTimeBlockViewModel>();



            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<PomodoroPage>();
            builder.Services.AddTransient<EisenhowerMatrixPage>();
            builder.Services.AddTransient<TimeBlockingPage>();
            builder.Services.AddTransient<AddTimeBlockPage>();
            builder.Services.AddTransient<EditTimeBlockPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            return app;
        }
    }
}
