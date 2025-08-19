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

        private void OnStartServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
            SleepTimer.Platforms.Android.ServiceHelper.StartVolumeService();
#endif
        }

        private void OnStopServiceClicked(object sender, EventArgs e)
        {
#if ANDROID
            SleepTimer.Platforms.Android.ServiceHelper.StopVolumeService();
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
