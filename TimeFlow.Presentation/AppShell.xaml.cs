using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));
            Routing.RegisterRoute(nameof(AddTimeBlockPage), typeof(AddTimeBlockPage));
            Routing.RegisterRoute(nameof(EditTimeBlockPage), typeof(EditTimeBlockPage));
        }
    }
}
