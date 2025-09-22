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
        public MainPageDisplay MainPageDisplay { get; }
        public DebugProperties DebugProperties { get; }
        readonly ISleepTimerServiceHelper sleepTimerServiceHelper;



        public MainVM(AppPreferences appPreferences,
            MainTimer mainTimer,
            MainPageDisplay mainPageDisplay,
            ISleepTimerServiceHelper sleepTimerServiceHelper,
            DebugProperties debugProperties
            )
        {
            this.AppPreferences = appPreferences;
            this.MainTimer = mainTimer;
            this.MainPageDisplay = mainPageDisplay;
            this.sleepTimerServiceHelper = sleepTimerServiceHelper;
            this.DebugProperties = debugProperties;

            App.Current!.UserAppTheme = AppTheme.Dark;
            Plugin.LocalNotification.LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;

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