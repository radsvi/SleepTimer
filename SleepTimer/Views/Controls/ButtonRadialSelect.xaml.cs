namespace SleepTimer.Views.Controls;

public partial class ButtonRadialSelect : ContentView
{
	public ButtonRadialSelect()
	{
		InitializeComponent();
        UpdateRightField();

    }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonRadialSelect), string.Empty);

    public string Units
    {
        get { return (string)GetValue(UnitsProperty); }
        set { SetValue(UnitsProperty, value); }
    }
    public static readonly BindableProperty UnitsProperty =
        BindableProperty.Create(nameof(Units), typeof(string), typeof(ButtonRadialSelect), string.Empty);

    //public string RightTextField
    //{
    //    get { return (string)GetValue(RightTextFieldProperty); }
    //    set { SetValue(RightTextFieldProperty, value); }
    //}
    //public static readonly BindableProperty RightTextFieldProperty =
    //    BindableProperty.Create(nameof(RightTextField), typeof(string), typeof(ButtonRadialSelect), string.Empty);

    public int Value
    {
        get { return (int)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(ButtonRadialSelect), 0);

    public int Minimum
    {
        get { return (int)GetValue(MinimumProperty); }
        set { SetValue(MinimumProperty, value); }
    }
    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(int), typeof(ButtonRadialSelect), 0);

    public int Maximum
    {
        get { return (int)GetValue(MaximumProperty); }
        set { SetValue(MaximumProperty, value); }
    }
    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(int), typeof(ButtonRadialSelect), 0);

    private string rightTextField = string.Empty;
    public string RightTextField { get => rightTextField; private set { rightTextField = value; OnPropertyChanged(); } }

    private async void OnGridTapped(object sender, EventArgs e)
    {
        //var route = $"{nameof(RadialSliderPage)}?{nameof(RadialSliderPage.Description)}={Text}";
        //var route = $"{nameof(RadialSliderPage)}";
        var route = $"{nameof(RadialSliderPage)}?" +
            $"{nameof(RadialSliderPage.Description)}={Uri.EscapeDataString(Text)}" +
            $"&{nameof(RadialSliderPage.PassValue)}={Uri.EscapeDataString(Value.ToString())}";
        await AppShell.Current.GoToAsync(route);

        UpdateRightField();
    }
    private void UpdateRightField()
    {
        RightTextField = $"{Value} {Units}";
    }
}