using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class SleepTimerOrchestrator
    {
        private readonly MainTimer timer;
        private readonly MediaController mediaController;
        private readonly TimerNotifier notifier;
        private readonly MainPageDisplay mainPageDisplay;
        private readonly INotificationManager notificationManager;
        private readonly AppPreferences appPreferences;

        public SleepTimerOrchestrator(
            MainTimer timer,
            AppPreferences preferences,
            IVolumeService volumeService,
            IMediaControlService mediaService,
            MainPageDisplay display,
            INotificationManager notificationManager)
        {
            this.timer = timer;
            this.appPreferences = preferences;
            this.mediaController = new MediaController(volumeService, mediaService, preferences);
            this.notifier = new TimerNotifier(preferences, (msg, level) => notificationManager.Update(msg, level));
            this.mainPageDisplay = display;
            this.notificationManager = notificationManager;

            WireUpEvents();
        }

        private void WireUpEvents()
        {
            mainPageDisplay.SetStartTime(appPreferences.TimerDurationSeconds);

            timer.Tick += (s, remaining) =>
            {
                notifier.OnTick(remaining);
                mainPageDisplay.OnTick(remaining);

                if (remaining.TotalSeconds <= appPreferences.FadeOutSeconds)
                    mediaController.HandleFadeOut(remaining);
                else
                    mediaController.SetStartingVolume();
            };

            timer.EnteredStandby += (s, e) => mediaController.EnterStandby();
            timer.Finished += (s, e) =>
            {
                timer.StopTimer();
                mediaController.HandleFinished();
            };
        }

        public void HandleIntent(Intent? intent)
        {
            if (intent?.Action == ServiceAction.Start.ToString())
            {
                timer.StartTimer(notificationManager.Update);
                notificationManager.Show($"Starting timer. {appPreferences.TimerDurationSeconds} minutes left.");
            }
            else if (intent?.Action == ServiceAction.Extend.ToString())
            {
                timer.Extend();
            }
            else if (intent?.Action == ServiceAction.Stop.ToString())
            {
                timer.StopTimer();
            }
        }

        public void Cleanup()
        {
            timer.StopTimer();
        }
    }
}
