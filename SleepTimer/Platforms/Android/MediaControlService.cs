using Android.Content;
using Android.Views;
using Android.App;

namespace SleepTimer.Platforms.Android
{
    public class MediaControlService : IMediaControlService
    {
        public void PauseOtherApps()
        {
            var context = global::Android.App.Application.Context;

            // Send Media Pause key events
            var downIntent = new Intent(Intent.ActionMediaButton);
            downIntent.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Down, Keycode.MediaPause));
            context.SendBroadcast(downIntent);

            var upIntent = new Intent(Intent.ActionMediaButton);
            upIntent.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Up, Keycode.MediaPause));
            context.SendBroadcast(upIntent);
        }
    }
}
