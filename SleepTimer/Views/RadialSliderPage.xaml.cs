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
            vm.SliderSize = Math.Min(width, height) * 0.8f;
        }
    }
}