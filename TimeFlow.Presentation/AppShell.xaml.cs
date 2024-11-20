using TimeFlow.Presentation.Views;

namespace TimeFlow.Presentation
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EisenhowerMatrixPage), typeof(EisenhowerMatrixPage));
        }
    }
}
