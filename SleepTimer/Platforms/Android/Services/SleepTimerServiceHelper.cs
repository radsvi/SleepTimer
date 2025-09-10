using Android.Content;
using Android.OS;

namespace SleepTimer.Platforms.Android.Services
{
    public class SleepTimerServiceHelper : ISleepTimerServiceHelper
    {
        public void SleepTimerControl(ServiceAction action)
        {
            var context = global::Android.App.Application.Context ?? throw new NullReferenceException();
            var intent = new Intent(context, typeof(SleepTimerService));
            intent.SetAction(action.ToString());

            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
#pragma warning disable CA1416
                context.StartForegroundService(intent);
#pragma warning restore CA1416
            else
                context.StartService(intent);

        }

    }
}
