using TimeFlow.Presentation.ViewModels.Popups;

namespace TimeFlow.Presentation.Views.Popups
{
    public partial class AddHabitPopup : CommunityToolkit.Maui.Views.Popup
    {
        public AddHabitPopup(AddHabitPopupViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            viewModel.OnPopupClosed += () => this.Close();
        }
    }
}
