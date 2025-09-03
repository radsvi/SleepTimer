using Android.Content;
//using Android.Views;
using Android.Media.Session;
using Android.App;

namespace SleepTimer.Platforms.Android
{
    public class MediaControlService : IMediaControlService
    {
        public void StopPlayback()
        {
            var context = global::Android.App.Application.Context;

            var mediaSessionManager = (MediaSessionManager?)context.GetSystemService(Context.MediaSessionService)
                ?? throw new InvalidOperationException("MediaSessionManager not available"); ;

            if (!NotificationAccessHelper.HasNotificationAccess(context))
            {
                NotificationAccessHelper.RequestNotificationAccess(context); // Show a dialog/snackbar to explain to the user

                return;
            }

            var sessions = mediaSessionManager.GetActiveSessions(null);

            foreach (var session in sessions)
            {
                try
                {
                    var controller = session as MediaController;
                    controller?.GetTransportControls()?.Pause();
                }
                catch {}
            }
        }
    }
}
