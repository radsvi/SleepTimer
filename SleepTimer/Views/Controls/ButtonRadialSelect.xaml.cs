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

    public string Subtitle
    {
        get { return (string)GetValue(SubtitleProperty); }
        set { SetValue(SubtitleProperty, value); OnPropertyChanged(nameof(MiddleTextField)); }
    }
    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(ButtonRadialSelect), string.Empty);

    public string Units
    {
        get { return (string)GetValue(UnitsProperty); }
        set { SetValue(UnitsProperty, value); }
    }
    public static readonly BindableProperty UnitsProperty =
        BindableProperty.Create(nameof(Units), typeof(string), typeof(ButtonRadialSelect), string.Empty, propertyChanged: (b, o, n) => ((ButtonRadialSelect)b).UpdateRightField());

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
        BindableProperty.Create(nameof(Value), typeof(int), typeof(ButtonRadialSelect), 0, propertyChanged: (b, o, n) => ((ButtonRadialSelect)b).UpdateRightField());

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
        BindableProperty.Create(nameof(Maximum), typeof(int), typeof(ButtonRadialSelect), 60);

    private string rightTextField = string.Empty;
    public string RightTextField { get => rightTextField; private set { rightTextField = value; OnPropertyChanged(); } }
    public string MiddleTextField { get => (String.IsNullOrEmpty(Subtitle)) ? string.Empty : (string)$"({Subtitle})"; }

    private async void OnGridTapped(object sender, EventArgs e)
    {
        var tcs = new TaskCompletionSource<double>();
        Routing.RegisterRoute(nameof(RadialSliderPage), typeof(RadialSliderPage));

        var route = $"{nameof(RadialSliderPage)}?" +
            $"{nameof(RadialSliderVM.Description)}={Uri.EscapeDataString(Text)}" +
            $"&{nameof(RadialSliderVM.Subtitle)}={Uri.EscapeDataString(Subtitle)}" +
            $"&{nameof(RadialSliderVM.Units)}={Uri.EscapeDataString(Units)}" +
            $"&{nameof(RadialSliderVM.PassValue)}={Uri.EscapeDataString(Value.ToString())}" +
            $"&{nameof(RadialSliderVM.PassMinimum)}={Uri.EscapeDataString(Minimum.ToString())}" +
            $"&{nameof(RadialSliderVM.PassMaximum)}={Uri.EscapeDataString(Maximum.ToString())}";

        ResultPassingHelper.CurrentTCS = tcs;

        await AppShell.Current.GoToAsync(route);

        try
        {
            double result = await tcs.Task;
            Value = (int)Math.Round(result);
            System.Diagnostics.Debug.WriteLine($"## Result: {result}");
        }
        catch (TaskCanceledException)
        {
            System.Diagnostics.Debug.WriteLine("## Cancelled");
        }
    }
    private void UpdateRightField()
    {
        RightTextField = $"{Value} {Units}";
    }

}