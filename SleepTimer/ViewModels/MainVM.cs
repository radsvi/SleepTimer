using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    public partial class MainVM(AppPreferences appPreferences) : ObservableObject
    {
        public AppPreferences AppPreferences { get; } = appPreferences;


        [RelayCommand]
        async Task NavigateToPage(Pages page)
        {
            //string destination;
            //switch (menuItem)
            //{
            //    case PopupParams.OptionsPage:
            //        destination = nameof(OptionsPage);
            //        break;
            //    //case PopupParams.LoadLevelPage:
            //    //    destination = nameof(LoadLevelPage);
            //    //    break;
            //    default:
            //        return;
            //}

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
    }
}
