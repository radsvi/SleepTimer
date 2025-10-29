namespace SleepTimer.Views;

public partial class SleepLogsPage : ContentPage
{
	public SleepLogsPage(SleepLogsVM sleepLogsVM)
	{
		InitializeComponent();

		BindingContext = sleepLogsVM;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

		if (BindingContext is SleepLogsVM vm)
            vm.OnAppearing();
    }
}