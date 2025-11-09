using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.ViewModels
{
    [QueryProperty(nameof(RadialSliderVM.Description), nameof(Description))]
    [QueryProperty(nameof(RadialSliderVM.Subtitle), nameof(Subtitle))]
    [QueryProperty(nameof(RadialSliderVM.Units), nameof(Units))]
    [QueryProperty(nameof(RadialSliderVM.PassValue), nameof(PassValue))]
    [QueryProperty(nameof(RadialSliderVM.PassMinimum), nameof(PassMinimum))]
    [QueryProperty(nameof(RadialSliderVM.PassMaximum), nameof(PassMaximum))]
    public partial class RadialSliderVM : ObservableObject
    {
        private string description = string.Empty;
        private string subtitle = string.Empty;
        private string units = string.Empty;
        private double value;
        private int minimum;
        private int maximum;

        public string Description { get => description; set { description = value; OnPropertyChanged(); } }
        public string Subtitle { get => subtitle; set { subtitle = value; OnPropertyChanged(); } }
        public string Units { get => units; set { units = value; OnPropertyChanged(); } }
        public double Value { get => value; set { this.value = value; OnPropertyChanged(); } }
        public int Minimum { get => minimum; set { minimum = value; OnPropertyChanged(); } }
        public int Maximum { get => maximum; set { maximum = value; OnPropertyChanged(); } }

        public string PassValue { get; set; } = string.Empty;
        public string PassMinimum { get; set; } = string.Empty;
        public string PassMaximum { get; set; } = string.Empty;




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
            ParseValues();
        }
        private void ParseValues()
        {
            {
                var successValue = double.TryParse(PassValue, out var resultValue);
                if (!successValue)
                    throw new Exception($"Parsing error. PassValue: {PassValue}");

                Value = resultValue;
            }
            {
                var successMinimum = int.TryParse(PassMinimum, out var resultMinimum);
                if (!successMinimum)
                    throw new Exception($"Parsing error. PassMinimum: {PassMinimum}");

                Minimum = resultMinimum;
            }
            {
                var successMaximum = int.TryParse(PassMaximum, out var resultMaximum);
                if (!successMaximum)
                    throw new Exception($"Parsing error. PassMaximum: {PassMaximum}");

                Maximum = resultMaximum;
            }
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
