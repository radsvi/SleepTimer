using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.Service.Notification;
using Android.Provider;
using Android.OS;
using SleepTimer.Platforms.Android.Services;
using Android;
using Android.Content.PM;
using AndroidX.Core.Content;

namespace SleepTimer.Platforms.Android
{
    public class MediaControlService : IMediaControlService
    {
        public void StopPlayback()
        {
            var context = global::Android.App.Application.Context;

            
            //if (!HasNotificationAccess())
            //{
            //    RequestNotificationAccess();
            //    return; // Interrupting here. User has to grant access manually.
            //}
            
            var mediaSessionManager = (MediaSessionManager)context.GetSystemService(Context.MediaSessionService);
            var componentName = new ComponentName(context, Java.Lang.Class.FromType(typeof(NotificationListener)).Name);
            var sessions = mediaSessionManager.GetActiveSessions(componentName);

            foreach (var controller in sessions)
            {
                try
                {
                    controller?.GetTransportControls()?.Pause();
                }
                catch {}
            }
        }
    }
}
