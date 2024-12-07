using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.Maui.Audio;
using System.Diagnostics;
using TimeFlow.Core.Interfaces;
using TimeFlow.Core.Services;
using TimeFlow.Domain.Entities;
using TimeFlow.Infrastructure.Data;
using TimeFlow.Infrastructure.Repositories;
using TimeFlow.Presentation.Services;
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
                .UseMauiCommunityToolkit()
                .AddAudio()
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

            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<IPomodoroSessionRepository, PomodoroSessionRepository>();
            builder.Services.AddScoped<IHabitRepository, HabitRepository>();

            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IHabitService, HabitService>();

#if ANDROID || IOS
            builder.Services.AddSingleton<INotifyService, AndroidNotificationService>();
#elif WINDOWS
            builder.Services.AddSingleton<INotifyService, WindowsNotificationService>();
#endif

            // Регистрация ViewModel и страниц
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<PomodoroViewModel>();
            builder.Services.AddTransient<EisenhowerMatrixViewModel>();
            builder.Services.AddTransient<HabitTrackerViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<PomodoroPage>();
            builder.Services.AddTransient<EisenhowerMatrixPage>();
            builder.Services.AddTransient<HabitTrackerPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            return app;
        }
    }
}
