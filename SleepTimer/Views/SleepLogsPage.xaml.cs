namespace SleepTimer.Views;

public partial class SleepLogsPage : ContentPage
{
	public SleepLogsPage(SleepLogsVM sleepLogsVM)
	{
		InitializeComponent();

		BindingContext = sleepLogsVM;
	}
}