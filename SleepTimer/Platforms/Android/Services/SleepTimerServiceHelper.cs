using Android.Content;
using Android.OS;

namespace SleepTimer.Platforms.Android.Services
{
    public class SleepTimerServiceHelper : ISleepTimerServiceHelper
    {
        public void SleepTimerControl(ServiceAction action)
        {
            var context = global::Android.App.Application.Context;
            var intent = new Intent(context, typeof(SleepTimerService));
            intent.SetAction(action.ToString());

            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
                context.StartForegroundService(intent);
            else
                context.StartService(intent);

        }

    }
}
