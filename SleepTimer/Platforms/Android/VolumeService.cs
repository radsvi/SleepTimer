using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using System.Timers;

namespace SleepTimer.Platforms.Android
{

    public class VolumeService : IVolumeService
    {
        private readonly AudioManager audioManager = (AudioManager?)global::Android.App.Application.Context.GetSystemService(Context.AudioService)
            ?? throw new InvalidOperationException("AudioService not available");
        private readonly AppPreferences appPreferences = ServiceHelper.GetService<AppPreferences>();
        //public void LowerVolume()
        //{
        //    int targetVolume = 0;
        //    int i = 0;

        //    while (GetVolume() > targetVolume)
        //    {
        //        // Simulate user volume button presses
        //        audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, VolumeNotificationFlags.ShowUi);
        //        //audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, Adjust.Lower, 0); // hide UI

        //        Task.Delay(200).Wait();
        //        i++;
        //        if (i >= 100)
        //            throw new InvalidOperationException("Couldn't lower the volume.");
        //    }
        //}
        public async void SetVolume(int targetVolume)
        {
            // Note: 100% volume equals to value 16 on Android
            if (targetVolume == GetVolume()
                || targetVolume > 16
                || targetVolume < 0)
                return;

            var action = targetVolume < GetVolume() ? Adjust.Lower : Adjust.Raise;
            VolumeNotificationFlags showUi = appPreferences.DisplayVolumeChange ? VolumeNotificationFlags.ShowUi : 0;

            int i = 0;
            while (LoopCondition(action, targetVolume))
            {
                // Simulate user volume button presses
                audioManager.AdjustStreamVolume(global::Android.Media.Stream.Music, action, showUi);

                await Task.Delay(200);
                i++;
                if (i >= 100)
                    throw new InvalidOperationException("Couldn't set the volume.");
            }
        }
        private bool LoopCondition(Adjust action, int targetVolume)
        {
            if ((action == Adjust.Lower && targetVolume < GetVolume())
                || (action == Adjust.Raise && targetVolume > GetVolume()))
                return true;

            return false;
        }
        public int GetVolume()
        {
            return this.audioManager.GetStreamVolume(global::Android.Media.Stream.Music);
        }
    }
}
