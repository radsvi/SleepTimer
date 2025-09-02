namespace SleepTimer.Views.Controls;

public partial class SwitchControl : ContentView
{
	public SwitchControl()
	{
		InitializeComponent();

        this.ControlTemplate = this.ControlTemplate;
        this.Loaded += OnControlLoaded;
    }

	public bool IsChecked
	{
		get { return (bool)GetValue(IsCheckedProperty); }
		set { SetValue(IsCheckedProperty, value); }
	}
	public static readonly BindableProperty IsCheckedProperty =
		BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(SwitchControl), false);

	public string Text
	{
		get { return (string)GetValue(TextProperty); }
		set { SetValue(TextProperty, value); }
	}
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(SwitchControl), string.Empty);

    private void OnControlLoaded(object sender, EventArgs e)
    {
        if (this.GetTemplateChild("LinkedLabel") is Label label &&
            this.GetTemplateChild("LinkedCheckBox") is CheckBox checkBox)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, args) =>
            {
                checkBox.IsChecked = !checkBox.IsChecked;
            };

            label.GestureRecognizers.Add(tap);
        }
    }
}