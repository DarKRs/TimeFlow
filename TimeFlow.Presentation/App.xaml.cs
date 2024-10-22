using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Устанавливаем MainPage после инициализации ресурсов
            MainPage = _serviceProvider.GetRequiredService<MainPage>();

            MainPage = new AppShell();
        }
    }
}
