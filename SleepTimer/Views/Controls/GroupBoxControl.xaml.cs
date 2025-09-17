namespace SleepTimer.Views.Controls;

public partial class GroupBoxControl : ContentView
{

    public GroupBoxControl()
    {
        InitializeComponent();
    }
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(GroupBoxControl), string.Empty);

}