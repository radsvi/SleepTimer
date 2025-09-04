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
        readonly IAudioFocusHelper audioFocusHelper;
        readonly IMediaControlService mediaService;
        readonly IMediaPlaybackBroadcast playbackBroadcasts;
        readonly IGradualVolumeService gradualVolumeService;

        public MainVM(AppPreferences appPreferences, MainTimer mainTimer, IMediaControlService mediaService, IAudioFocusHelper audioFocusHelper, IMediaPlaybackBroadcast mediaPlaybackBroadcast, IGradualVolumeService gradualVolumeService)
        {
            AppPreferences = appPreferences;
            MainTimer = mainTimer;

            //MainTimer.PropertyChanged += MainTimer_PropertyChanged;
            this.mediaService = mediaService;
            this.audioFocusHelper = audioFocusHelper;
            this.playbackBroadcasts = mediaPlaybackBroadcast;
            this.gradualVolumeService = gradualVolumeService;

            Plugin.LocalNotification.LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
        }

        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            MainTimer.Extend();
        }

        #region Methods
        //private void MainTimer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(MainTimer.IsStarted))
        //    {
        //        ExtendTimerCommand.NotifyCanExecuteChanged();
        //    }
        //}
        
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
        //[RelayCommand]
        //void StartTimer()
        //{
        //    MainTimer.StartTimer();
        //}
        //[RelayCommand]
        //void StopTimer()
        //{
        //    MainTimer.StopTimer();
        //}
        //[RelayCommand(CanExecute = nameof(IsStarted))]
        //void ExtendTimer()
        //{
        //    MainTimer.Extend();
        //}
        bool IsStarted()
        {
            return MainTimer.IsStarted;
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
        #endregion
    }
}
