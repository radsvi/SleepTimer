namespace SleepTimer.Views;

public partial class RadialSliderPage : ContentPage
{

    public RadialSliderPage(RadialSliderVM vm)
	{
		InitializeComponent();

        BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RadialSliderVM vm)
        {
            vm.OnViewAppearing();
        }
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (BindingContext is RadialSliderVM vm)
        {
            vm.SliderSize = Math.Min(width, height);
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (ResultPassingHelper.CurrentTCS is { Task.IsCompleted: false })
        {
            ResultPassingHelper.CurrentTCS.TrySetCanceled();
        }

        ResultPassingHelper.CurrentTCS = null;
    }
}