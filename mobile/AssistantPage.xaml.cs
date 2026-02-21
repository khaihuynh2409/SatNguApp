using SatNguApp.Mobile.ViewModels;

namespace SatNguApp.Mobile;

public partial class AssistantPage : ContentPage
{
	public AssistantPage()
	{
		InitializeComponent();
        BindingContext = new AssistantViewModel();
	}
}
