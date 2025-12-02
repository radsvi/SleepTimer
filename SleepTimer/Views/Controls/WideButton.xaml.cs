namespace SleepTimer.Views.Controls;

public partial class WideButton : ContentView
{
	public WideButton()
	{
		InitializeComponent();
	}



	public string Text
	{
		get { return (string)GetValue(TextProperty); }
		set { SetValue(TextProperty, value); }
	}
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(WideButton), string.Empty);

	public string RightText
	{
		get { return (string)GetValue(RightTextProperty); }
		set { SetValue(RightTextProperty, value); }
	}
	public static readonly BindableProperty RightTextProperty =
		BindableProperty.Create(nameof(RightText), typeof(string), typeof(WideButton), string.Empty);

    //public Command Command
    //{
    //    get { return (Command)GetValue(CommandProperty); }
    //    set { SetValue(CommandProperty, value); }
    //}
    //public static readonly BindableProperty CommandProperty =
    //    BindableProperty.Create(nameof(Command), typeof(Command), typeof(WideButton));

    //public object CommandParameter
    //{
    //    get { return (object)GetValue(CommandParameterProperty); }
    //    set { SetValue(CommandParameterProperty, value); }
    //}
    //public static readonly BindableProperty CommandParameterProperty =
    //    BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(WideButton));

    public Pages NavigateTo
    {
        get { return (Pages)GetValue(NavigateToProperty); }
        set { SetValue(NavigateToProperty, value); }
    }
    public static readonly BindableProperty NavigateToProperty =
        BindableProperty.Create(nameof(NavigateTo), typeof(Pages), typeof(WideButton));




    private async void OnGridTapped(object sender, EventArgs e)
    {
        string route;
        if (NavigateTo == Pages.MainPage)
        {
            route = "///" + NavigateTo.ToString();
        }
        else
        {
            route = NavigateTo.ToString();
        }

        if (Shell.Current.FlyoutIsPresented is true)
            Shell.Current.FlyoutIsPresented = false;
        await AppShell.Current.GoToAsync(route);
    }

}