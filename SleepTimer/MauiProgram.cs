using Microsoft.Extensions.Logging;
//using Syncfusion.Maui.Core.Hosting;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using SleepTimer.Views.Controls;

namespace SleepTimer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    handlers.AddHandler(typeof(NumberPicker), typeof(SleepTimer.Platforms.Android.NumberPickerHandler));
//#elif IOS
//                    handlers.AddHandler(typeof(NumberPicker), typeof(NumberPickerHandler));
#endif
                })
                .UseLocalNotification(cfg =>
                {
                    cfg.AddAndroid(android =>
                    {
                        android.AddChannel(new NotificationChannelRequest
                        {
                            Id = "silent_channel",
                            Name = "Silent Notifications",
                            Description = "Notifications without sound",
                            Importance = AndroidImportance.Low, // Low = no sound
                            EnableVibration = false,           // Optional: no vibration
                        });
                    });
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<ConfigurationPage>();

            builder.Services.AddSingleton<MainVM>();
            builder.Services.AddSingleton<ConfigurationVM>();

            builder.Services.AddSingleton<AppPreferences>();
            builder.Services.AddSingleton<MainTimer>();


#if ANDROID
            builder.Services.AddSingleton<IMediaControlService, Platforms.Android.MediaControlService>();
            builder.Services.AddSingleton<ISleepTimerService, Platforms.Android.Services.SleepTimerService>();
            builder.Services.AddSingleton<IVolumeService, Platforms.Android.VolumeService>();
            builder.Services.AddSingleton<ISleepTimerServiceHelper, Platforms.Android.Services.SleepTimerServiceHelper>();
            builder.Services.AddSingleton<IPermissionHelper, Platforms.Android.PermissionHelper>();
#elif WINDOWS
            builder.Services.AddSingleton<IMediaControlService, Platforms.Windows.MediaControlService>();

#else
            builder.Services.AddSingleton<IMediaControlService, StubMediaControlService>();
            builder.Services.AddSingleton<ISleepTimerService, StubSleepTimerService>();
            builder.Services.AddSingleton<IVolumeService, StubVolumeService>();
            builder.Services.AddSingleton<ISleepTimerServiceHelper, Services.StubSleepTimerServiceHelper>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Enable Exception Logging
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[Unhandled] {e.ExceptionObject}");
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[UnobservedTask] {e.Exception}");
            };


            return builder.Build();
        }
    }
}
