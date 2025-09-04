//using Android.Gms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Plugin.LocalNotification;
#if ANDROID
using Android.Content;
using Android.OS;
using SleepTimer.Platforms.Android;
#endif


namespace SleepTimer.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        public AppPreferences AppPreferences { get; }
        public MainTimer MainTimer { get; }
        //public Notifications notifications { get; }
        readonly IVolumeService volumeService;
        readonly IAudioFocusHelper audioFocusHelper;
        readonly IMediaControlService mediaService;
        readonly IMediaPlaybackBroadcast playbackBroadcasts;

        public MainVM(AppPreferences appPreferences, MainTimer mainTimer, IVolumeService volumeService, IMediaControlService mediaService, IAudioFocusHelper audioFocusHelper, IMediaPlaybackBroadcast mediaPlaybackBroadcast)
        {
            AppPreferences = appPreferences;
            MainTimer = mainTimer;

            MainTimer.PropertyChanged += MainTimer_PropertyChanged;
            this.volumeService = volumeService;
            this.mediaService = mediaService;
            this.audioFocusHelper = audioFocusHelper;
            this.playbackBroadcasts = mediaPlaybackBroadcast;

            Plugin.LocalNotification.LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
        }

        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            MainTimer.Extend();
        }
        #region Methods
        private void MainTimer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainTimer.IsStarted))
            {
                ExtendTimerCommand.NotifyCanExecuteChanged();
            }
        }
        [RelayCommand]
        private void IncreaseVolume()
        {
            var current = volumeService.GetVolume();
            if (current >= 0)
                volumeService.SetVolume(current + 10);
        }
        [RelayCommand]
        private void DecreseVolume()
        {
            var current = volumeService.GetVolume();
            if (current >= 0)
                volumeService.SetVolume(current - 10);
        }

        
        [RelayCommand]
        async Task NavigateToPage(Pages page)
        {
            string route;
            if (page == Pages.MainPage)
            {
                route = "///" + page.ToString();
            }
            else
            {
                route = page.ToString();
            }

            if (Shell.Current.FlyoutIsPresented is true)
                Shell.Current.FlyoutIsPresented = false;
            await AppShell.Current.GoToAsync(route);
        }
        [RelayCommand]
        void StopPlayback()
        {
            //mediaService.StopPlayback();
            audioFocusHelper.RequestAudioFocus();
        }
        [RelayCommand]
        void StartTimer()
        {
            MainTimer.Start();
        }
        [RelayCommand]
        void StopTimer()
        {
            MainTimer.Stop();
        }
        [RelayCommand(CanExecute = nameof(IsStarted))]
        void ExtendTimer()
        {
            MainTimer.Extend();
        }
        bool IsStarted()
        {
            return MainTimer.IsStarted;
        }
        [RelayCommand]
        public void StartVolumeTimer()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            var intent = new Intent(context, typeof(VolumeAdjustService));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                context.StartForegroundService(intent);
            else
                context.StartService(intent);
#endif
        }
        [RelayCommand]
        public void StartSleepTimer()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            var intent = new Intent(context, typeof(SleepTimerService));
            intent.SetAction(ServiceAction.Start.ToString());
            //intent.PutExtra("minutes", 25); // start 15-minute timer
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                context.StartForegroundService(intent);
            else
                context.StartService(intent);
#endif
        }
        [RelayCommand]
        public void StopSleepTimer()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            var intent = new Intent(context, typeof(SleepTimerService));
            intent.SetAction(ServiceAction.Stop.ToString());
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                context.StartForegroundService(intent);
            else
                context.StartService(intent);
#endif
        }
        [RelayCommand]
        public void SendPauseBroadcast()
        {
            playbackBroadcasts.SendMediaPause();
        }
        [RelayCommand]
        public void SendStopBroadcast()
        {
            playbackBroadcasts.SendMediaStop();
        }
        #endregion
    }
}
