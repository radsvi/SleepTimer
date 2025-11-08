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
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (BindingContext is MainVM vm)
            {
                vm.SliderSize = Math.Min(width, height) * 0.8f;
            }
        }
    }
}
