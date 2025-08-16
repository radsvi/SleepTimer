using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SleepTimer.Models
{
    public class MainTimer : ObservableObject
    {
        readonly AppPreferences appPreferences;
        readonly IVolumeService volumeService;
        public MainTimer(AppPreferences appPreferences, IVolumeService volumeService)
        {
            this.appPreferences = appPreferences;
            this.volumeService = volumeService;

            //Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            Timer.Elapsed += OnTimedEvent;
            Timer.Interval = 1000; // seconds
        }
        public System.Timers.Timer Timer { get; private set; } = new System.Timers.Timer();
        public DateTime? EndTime { get; private set; }
        private bool isStarted;
        public bool IsStarted
        {
            get => isStarted;
            set { isStarted = value; OnPropertyChanged(); }
        }
        private TimeSpan remainingTime = TimeSpan.MinValue;
        public TimeSpan RemainingTime
        {
            get => remainingTime;
            set { remainingTime = value; OnPropertyChanged(); }
        }
        public int StartingVolume { get; private set; }
        [Obsolete]private int volumeTest = 90;
        [Obsolete]public int VolumeTest
        {
            get => volumeTest;
            set { volumeTest = value; OnPropertyChanged(); }
        }
        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            if (EndTime == null)
                return;

            RemainingTime = (DateTime)EndTime - e.SignalTime;

            if (RemainingTime.CompareTo(new TimeSpan(0,0,Constants.FinalPhaseSeconds)) < 0)
            {
                DecreseVolume();
            }
            else
            {
                // User can change the volume while the SleepTimer is active, but before the final phase is reached, and the starting volume is still being stored correctly.
                StartingVolume = volumeService.GetVolume(); 
            }
        }
        public void Start()
        {
            Timer.Enabled = true;
            IsStarted = true;
            EndTime = DateTime.Now.AddMinutes(appPreferences.DefaultDuration);

            if (EndTime != null)
                RemainingTime = (DateTime)EndTime - DateTime.Now;
        }
        public void Stop()
        {
            Timer.Enabled = false;
            IsStarted = false;
            EndTime = null;
        }
        public void Extend()
        {
            if (EndTime is null) throw new InvalidOperationException();

            DateTime dateTime = (DateTime)EndTime;

            EndTime = dateTime.AddMinutes(appPreferences.ExtensionLength);
        }
        private void DecreseVolume()
        {
            var currentVolume = volumeService.GetVolume();
            if (currentVolume <= 0)
                return;

            //var newVolume = currentVolume - Constants.VolumeStep;
            int newVolume = (100 * RemainingTime.Seconds / Constants.FinalPhaseSeconds);
            volumeService.SetVolume(newVolume);





            VolumeTest = newVolume;
        }
    }
}
