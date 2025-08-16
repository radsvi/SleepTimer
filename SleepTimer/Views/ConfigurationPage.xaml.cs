namespace SleepTimer.Views;

public partial class ConfigurationPage : ContentPage
{
	public ConfigurationPage(ConfigurationVM configurationVM)
	{
		InitializeComponent();

		BindingContext = configurationVM;
	}
}