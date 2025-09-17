namespace SleepTimer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ConfigurationPage), typeof(ConfigurationPage));
            Routing.RegisterRoute(nameof(SleepLogsPage), typeof(SleepLogsPage));
        }
    }
}
