using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using System.Timers;

namespace SleepTimer.Platforms.Android
{

    public class GradualVolumeService : IGradualVolumeService
    {
        private readonly AudioManager audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(Context.AudioService)
            ?? throw new InvalidOperationException("AudioService not available");
        public void LowerVolume()
        {
            int targetVolume = 0;
            int i = 0;

            while (GetVolume() > targetVolume)
            {
                // Simulate user volume button presses
                audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, VolumeNotificationFlags.ShowUi);
                //audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, 0); // hide UI

                Task.Delay(200).Wait();
                i++;
                if (i >= 100)
                    throw new InvalidOperationException("Couldn't lower the volume.");
            }
        }
        int GetVolume()
        {
            return this.audioManager.GetStreamVolume(global::Android.Media.Stream.Music);
        }
    }
}
