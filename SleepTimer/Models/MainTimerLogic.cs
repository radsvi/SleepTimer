using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SleepTimer.Models
{
    public interface IMainTimerLogic
    {
        void OnTimedEvent(object? source, ElapsedEventArgs e);
        //Task OnExtendEvent();
    }
    //public partial class MainTimerLogic(AppPreferences appPreferences) : ObservableObject, IMainTimerLogic
    //{
    //    private readonly AppPreferences appPreferences = appPreferences;
    //    public DateTime? EndTime { get; private set; }
    //    private TimeSpan remainingTime = TimeSpan.MinValue;
    //    public TimeSpan RemainingTime
    //    {
    //        get => remainingTime;
    //        set { remainingTime = value; OnPropertyChanged(); }
    //    }
    //    private bool isFinished;
    //    public bool IsFinished
    //    {
    //        get => isFinished;
    //        set { isFinished = value; OnPropertyChanged(); }
    //    }

    //    public async void OnTimedEvent(object? source, ElapsedEventArgs e)
    //    {
    //        if (EndTime == null)
    //            return;

    //        RemainingTime = (DateTime)EndTime - e.SignalTime;

    //        // Volume:
    //        if (RemainingTime.CompareTo(new TimeSpan(0, 0, -appPreferences.WaitTimeAfterFadeOut)) < 0)
    //        {
    //            IsFinished = true;
    //            volumeService.SetVolume(StartingVolume);
    //            Stop();
    //            mediaService.StopPlayback();
    //        }
    //        else if (RemainingTime.CompareTo(new TimeSpan(0, 0, Constants.FadeOutDuration)) < 0)
    //        {
    //            DecreaseVolume();
    //        }
    //        else
    //        {
    //            // User can change the volume while the SleepTimer is active, but before the final phase is reached, and the starting volume is still being stored correctly.
    //            StartingVolume = volumeService.GetVolume();
    //        }

    //        // Notifications:
    //        if (RemainingTime.Minutes == 0 && RemainingTime.Seconds < 10)
    //        {
    //            await Notifications.Show(new NotificationMessageGoingToSleep());
    //        }
    //        else if (RemainingTime.Seconds > 0 && Math.Abs(RemainingTime.Minutes - LastNotificationUpdate) > 0)
    //        {
    //            await Notifications.Show(new NotificationMessageRemainingTime(RemainingTime.Minutes));
    //            LastNotificationUpdate = RemainingTime.Minutes;
    //        }
    //    }
    //}
}
