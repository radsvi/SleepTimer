namespace SleepTimer.Views.Controls;

public partial class SwitchControl : ContentView
{
	public SwitchControl()
	{
		InitializeComponent();

        this.ControlTemplate = this.ControlTemplate; // Re-apply to ensure Template is loaded
        this.Loaded += OnControlLoaded;
    }

	public bool IsToggled
	{
		get { return (bool)GetValue(IsToggledProperty); }
		set { SetValue(IsToggledProperty, value); }
	}
	public static readonly BindableProperty IsToggledProperty =
		BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SwitchControl), false);

	public string Text
	{
		get { return (string)GetValue(TextProperty); }
		set { SetValue(TextProperty, value); }
	}
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(SwitchControl), string.Empty);

    private void OnControlLoaded(object? sender, EventArgs e)
    {
        if (this.GetTemplateChild("LinkedLabel") is Label label &&
            this.GetTemplateChild("LinkedSwitch") is Microsoft.Maui.Controls.Switch switchElement)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, args) =>
            {
                switchElement.IsToggled = !switchElement.IsToggled;
            };

            label.GestureRecognizers.Add(tap);
        }
    }
}