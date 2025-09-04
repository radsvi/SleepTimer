using Android.Content;
using Android.Views;
using Android.App;

namespace SleepTimer.Platforms.Android
{
    public class MediaPlaybackBroadcast : IMediaPlaybackBroadcast
    {
        public void SendMediaPause()
        {
            var keyEvent = new Intent(Intent.ActionMediaButton);
            keyEvent.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Down, Keycode.MediaPause));
            Platform.CurrentActivity.SendOrderedBroadcast(keyEvent, null);
            Task.Delay(500).Wait();
            keyEvent = new Intent(Intent.ActionMediaButton);
            keyEvent.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Up, Keycode.MediaPause));
            Platform.CurrentActivity.SendOrderedBroadcast(keyEvent, null);
        }

        public void SendMediaStop()
        {
            var keyEvent = new Intent(Intent.ActionMediaButton);
            keyEvent.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Down, Keycode.MediaStop));
            Platform.CurrentActivity.SendOrderedBroadcast(keyEvent, null);
            Task.Delay(500).Wait();
            keyEvent = new Intent(Intent.ActionMediaButton);
            keyEvent.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Up, Keycode.MediaStop));
            Platform.CurrentActivity.SendOrderedBroadcast(keyEvent, null);
        }
    }
}
