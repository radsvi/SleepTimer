using System.Threading.Tasks;

namespace SleepTimer.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainVM mainVM)
        {
            InitializeComponent();

            BindingContext = mainVM;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Hello!",
                Subtitle = "subtitle",
                Description = "This is a local notification from MAUI",
                ReturningData = "Some data",
                BadgeNumber = 42,
                
            };
            //Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
            //{
            //    ChannelId = "default",

            //}
            //ChannelId = "default",
            //Importance = AndroidImportance.High,
            //Priority = AndroidPriority.High,
            //Visibility = AndroidVisibilityType.Public

            //#if ANDROID
            //            notification.Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
            //            {
            //                ChannelId = "default",
            //                Priority = Plugin.LocalNotification.AndroidOptionPriority.High
            //            };
            //#endif

            //NotifyTime = DateTime.Now.AddSeconds(5) // Schedule for 5 seconds later
            //var qwer = LocalNotificationCenter.Current.AreNotificationsEnabled;
            await EnsureNotificationPermissionAsync();
            //await LocalNotificationCenter.Current.RequestNotificationPermission();
            LocalNotificationCenter.Current.Show(notification);
        }
        public async Task<bool> EnsureNotificationPermissionAsync()
        {
            // Returns true if notifications are allowed for your app
            var enabled = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (enabled) return true;

            // Android 13+ shows a runtime prompt; this triggers it.
            // (Optionally request exact alarm if you schedule exact times)
            var granted = await LocalNotificationCenter.Current.RequestNotificationPermission(
                new NotificationPermission
                {
                    Android = { RequestPermissionToScheduleExactAlarm = false }
                });

            return granted;
        }
    }
}
