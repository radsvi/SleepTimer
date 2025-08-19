//using Android.Gms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Plugin.LocalNotification;


namespace SleepTimer.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        public AppPreferences AppPreferences { get; }
        public MainTimer MainTimer { get; }
        //public Notifications notifications { get; }
        readonly IVolumeService volumeService;
        readonly IMediaControlService mediaService;

        public MainVM(AppPreferences appPreferences, MainTimer mainTimer, IVolumeService volumeService, IMediaControlService mediaService)
        {
            AppPreferences = appPreferences;
            MainTimer = mainTimer;

            MainTimer.PropertyChanged += MainTimer_PropertyChanged;
            this.volumeService = volumeService;
            this.mediaService = mediaService;

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
            //notification.Show(5);
            await Notifications.Show(NotificationMsg.GoingToSleep);
        }
        [RelayCommand]
        async Task UpdateNotification()
        {
            //notification.Show(5);
            //await Notifications.Show(5);
        }
        [RelayCommand]
        static void CancelNotification()
        {
            //notification.Show(5);
            Notifications.Cancel();
        }

        #endregion
    }
}
