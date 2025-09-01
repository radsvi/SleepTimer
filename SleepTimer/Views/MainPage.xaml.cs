using Plugin.LocalNotification;
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
        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();

        //    // Example: request location permission
        //    var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

        //    if (status != PermissionStatus.Granted)
        //    {
        //        status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        //    }

        //    if (status != PermissionStatus.Granted)
        //    {
        //        await DisplayAlert("Permission Required", "Location is needed for this app to work properly.", "OK");
        //    }
        //}
        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    await Task.Run(() => EnsureNotificationPermissionAsync());
        //}
        //static async Task<bool> EnsureNotificationPermissionAsync()
        //{
        //    var enabled = await LocalNotificationCenter.Current.AreNotificationsEnabled();
        //    if (enabled)
        //        return true;

        //    var granted = await LocalNotificationCenter.Current.RequestNotificationPermission(
        //        new NotificationPermission
        //        {
        //            Android = { RequestPermissionToScheduleExactAlarm = false }
        //        });

        //    return granted;
        //}
        private void OnStartServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
            //SleepTimer.Platforms.Android.ServiceHelper.StartVolumeService();
#endif
        }

        private void OnStopServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
            //SleepTimer.Platforms.Android.ServiceHelper.StopVolumeService();
#endif
        }
        private async void OnSetVolumeClicked(object sender, EventArgs e)
        {
            await Task.Delay(5000);
#if ANDROID

            SleepTimer.Platforms.Android.VolumeForegroundService.Instance?.SetVolume(50);
#endif
        }
    }
}
