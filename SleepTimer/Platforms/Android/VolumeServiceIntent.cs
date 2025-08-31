using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;

namespace SleepTimer.Platforms.Android
{
    [Service(Enabled = true)]
    public class VolumeServiceIntent : Service
    {
        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Get volume from intent
            double volume = intent.GetDoubleExtra("volume", 0.5);

            // Change volume
            var audioManager = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService)!;
            int max = audioManager.GetStreamMaxVolume(global::Android.Media.Stream.System);
            int newVol = (int)(max * volume);
            audioManager.RequestAudioFocus(null, global::Android.Media.Stream.System, AudioFocus.GainTransient);
            audioManager.SetStreamVolume(global::Android.Media.Stream.System, newVol, VolumeNotificationFlags.ShowUi);
            audioManager.AbandonAudioFocus(null);
            //audioManager.SetStreamVolume(global::Android.Media.Stream.System, newVol, VolumeNotificationFlags.RemoveSoundAndVibrate);

            // Stop service after task
            StopSelf();

            return StartCommandResult.NotSticky;
        }
    }
}
