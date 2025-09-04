namespace SleepTimer
{
    public partial class App : Application
    {
        readonly IMediaControlService mediaControlService;
        public App(IMediaControlService mediaControlService)
        {
            InitializeComponent();

            this.mediaControlService = mediaControlService;
        }
        protected override void OnResume()
        {
            base.OnResume();

            mediaControlService.CheckNotificationAccess();
        }
        protected override void OnStart()
        {
            base.OnResume();

            mediaControlService.CheckNotificationAccess();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            //2400x1080 ratio
            //const int newHeight = 900;
            //const int newWidth = 450;
            const int newHeight = 1000;
            const int newWidth = 450;

            var resolution = DeviceDisplay.Current.MainDisplayInfo;

            var newWindow = new Window(new AppShell())
            {
                Height = newHeight,
                Width = newWidth,
                X = resolution.Width - newWidth,
                Y = 0,
            };

            return newWindow;
        }
    }
}