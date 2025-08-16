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
        private readonly IVolumeService volumeService;

        public MainVM(AppPreferences appPreferences, MainTimer mainTimer, IVolumeService volumeService)
        {
            AppPreferences = appPreferences;
            MainTimer = mainTimer;

            MainTimer.PropertyChanged += MainTimer_PropertyChanged;
            this.volumeService = volumeService;
        }

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

        #region Navigation
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
        #endregion
    }
}
