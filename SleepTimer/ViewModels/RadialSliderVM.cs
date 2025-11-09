using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    [QueryProperty(nameof(Description), nameof(Description))]
    [QueryProperty(nameof(PassValue), nameof(PassValue))]
    public partial class RadialSliderVM : ObservableObject
    {
        private string description;
        private double value;

        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public double Value { get => value; set { this.value = value; OnPropertyChanged(); } }
        public string PassValue { get; set; }




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
        private async Task CancelSelection()
        {
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        private async Task ConfirmSelection()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
