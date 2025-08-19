//using Android.Gms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.LocalNotification.AndroidOption;

namespace SleepTimer.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        public AppPreferences AppPreferences { get; }
        public MainTimer MainTimer { get; }
        readonly IVolumeService volumeService;
        readonly IMediaControlService mediaService;

        public MainVM(AppPreferences appPreferences, MainTimer mainTimer, IVolumeService volumeService, IMediaControlService mediaService)
        {
            AppPreferences = appPreferences;
            MainTimer = mainTimer;

            MainTimer.PropertyChanged += MainTimer_PropertyChanged;
            this.volumeService = volumeService;
            this.mediaService = mediaService;

            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
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
        //[RelayCommand]
        //void StopPlayback()
        //{
        //    mediaService.PauseOtherApps();
        //}
        [RelayCommand]
        async Task ShowNotification()
        {
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Sleep timer!",
                Description = "X minutes left. Tap to extend!",
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                {
                    Ongoing = true,
                    AutoCancel = false,
                    ChannelId = "default",
                    Priority = AndroidPriority.High
                }
            };

            await EnsureNotificationPermissionAsync();

            await LocalNotificationCenter.Current.Show(notification);
        }
        public async Task<bool> EnsureNotificationPermissionAsync()
        {
            var enabled = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (enabled)
                return true;

            var granted = await LocalNotificationCenter.Current.RequestNotificationPermission(
                new NotificationPermission
                {
                    Android = { RequestPermissionToScheduleExactAlarm = false }
                });

            return granted;
        }

        #endregion
    }
}
