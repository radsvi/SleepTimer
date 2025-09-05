using Plugin.LocalNotification;
using System.Threading.Tasks;

namespace SleepTimer.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainVM mainVM)
        {
            InitializeComponent();

            BindingContext = mainVM;

            DrawableExtractor.SaveSystemDrawables();
        }
    }
}
