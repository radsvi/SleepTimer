using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    [QueryProperty(nameof(Description), nameof(Description))]
    [QueryProperty(nameof(PassValue), nameof(PassValue))]
    [QueryProperty(nameof(Units), nameof(Units))]
    public partial class RadialSliderVM : ObservableObject
    {
        private string description = string.Empty;
        private double value;
        private string units = string.Empty;

        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public double Value { get => value; set { this.value = value; OnPropertyChanged(); } }
        public string Units { get => units; set { units = value; OnPropertyChanged(); } }

        public string PassValue { get; set; } = string.Empty;




        private double sliderSize;
        public double SliderSize
        {
            get => sliderSize;
            set
            {
                if (sliderSize == value) return;
                sliderSize = value;
                OnPropertyChanged();
            }
        }

        public void OnViewAppearing()
        {
            var success = double.TryParse(PassValue, out var result);
            if (success)
                Value = result;
        }
        [RelayCommand]
        private static async Task CancelSelection()
        {
            ResultPassingHelper.CurrentTCS?.TrySetCanceled();
            ResultPassingHelper.CurrentTCS = null;

            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        private async Task ConfirmSelection()
        {
            ResultPassingHelper.CurrentTCS?.TrySetResult(Value);
            ResultPassingHelper.CurrentTCS = null;

            await Shell.Current.GoToAsync("..");
        }
    }
}
