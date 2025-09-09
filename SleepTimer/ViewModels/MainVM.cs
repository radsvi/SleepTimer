using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        public AppPreferences AppPreferences { get; }
        public MainTimer MainTimer { get; }
        public MainPageDisplay FrontendDisplay { get; }
        readonly IMediaControlService mediaService;
        readonly IVolumeService volumeService;
        readonly ISleepTimerServiceHelper sleepTimerServiceHelper;



        public MainVM(AppPreferences appPreferences,
            MainTimer mainTimer,
            MainPageDisplay frontendDisplay,
            IMediaControlService mediaService,
            IVolumeService volumeService,
            ISleepTimerServiceHelper sleepTimerServiceHelper
            )
        {
            this.AppPreferences = appPreferences;
            this.MainTimer = mainTimer;
            this.FrontendDisplay = frontendDisplay;
            this.mediaService = mediaService;
            this.volumeService = volumeService;
            this.sleepTimerServiceHelper = sleepTimerServiceHelper;

            App.Current!.UserAppTheme = AppTheme.Dark;
            Plugin.LocalNotification.LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
            AppPreferences.PropertyChanged += PropertyChangedHandler;
            MainTimer.PropertyChanged += PropertyChangedHandler;
        }

        private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainTimer.InStandby) || e.PropertyName == nameof(AppPreferences.StandByDuration))
            {
                OnPropertyChanged(nameof(TextBelowTimer));
            }
        }
        public string TextBelowTimer
        {
            get
            {
                if (MainTimer.InStandby) return $"standby ({AppPreferences.StandByDuration} seconds)";
                else return "minutes remaining";
            }
        }
        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            MainTimer.Extend();
        }

        #region Methods
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
        public void StartSleepTimer()
        {
            sleepTimerServiceHelper.SleepTimerControl(ServiceAction.Start);
        }
        [RelayCommand]
        public void StopSleepTimer()
        {
            sleepTimerServiceHelper.SleepTimerControl(ServiceAction.Stop);
        }
        #endregion
    }
}