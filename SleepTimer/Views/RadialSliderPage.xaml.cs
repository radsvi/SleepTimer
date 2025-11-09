namespace SleepTimer.Views;

[QueryProperty(nameof(Description), nameof(Description))]
[QueryProperty(nameof(PassValue), nameof(PassValue))]
public partial class RadialSliderPage : ContentPage
{
    private string description;
    private double value;

    public string Description { get => description; set { description = value; OnPropertyChanged(); } }
    public double Value { get => value; set { this.value = value; OnPropertyChanged(); } }

    public string PassValue { get; set; }

#warning // predelat na dynamic? to bude asi bordel
    public RadialSliderPage()
	{
		InitializeComponent();

        BindingContext = this;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //var success = int.TryParse(Title, out var result);
        //if (!success)
        //    throw new InvalidOperationException("Parse error");

        var success = double.TryParse(PassValue, out var result);
        if (success)
            Value = result;

#warning // zbytek
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        var size = Math.Min(width, height) * 0.8f;
        MainRadialSlider.WidthRequest = size;
        MainRadialSlider.HeightRequest = size;
    }

    private async void OK_Button_Clicked(object sender, EventArgs e)
    {
        
    }
    private async void Cancel_Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}