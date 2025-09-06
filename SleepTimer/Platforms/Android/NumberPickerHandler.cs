using Android.Widget;
using Microsoft.Maui.Handlers;
using SleepTimer.Views.Controls;

namespace SleepTimer.Platforms.Android
{
    public class NumberPickerHandler : ViewHandler<SleepTimer.Views.Controls.NumberPicker, global::Android.Widget.NumberPicker>
    {
        public static IPropertyMapper<SleepTimer.Views.Controls.NumberPicker, NumberPickerHandler> Mapper =
            new PropertyMapper<SleepTimer.Views.Controls.NumberPicker, NumberPickerHandler>(ViewHandler.ViewMapper)
            {
                [nameof(SleepTimer.Views.Controls.NumberPicker.Minimum)] = MapMinimum,
                [nameof(SleepTimer.Views.Controls.NumberPicker.Maximum)] = MapMaximum,
                [nameof(SleepTimer.Views.Controls.NumberPicker.Value)] = MapValue
            };
        public NumberPickerHandler() : base(Mapper)
        {
        }
        protected override global::Android.Widget.NumberPicker CreatePlatformView()
        {
            var picker = new global::Android.Widget.NumberPicker(Context);
            return picker;
        }

        protected override void ConnectHandler(global::Android.Widget.NumberPicker platformView)
        {
            base.ConnectHandler(platformView);

            platformView.MinValue = VirtualView.Minimum;
            platformView.MaxValue = VirtualView.Maximum;
            platformView.Value = VirtualView.Value;

            platformView.ValueChanged += OnValueChanged;
        }

        protected override void DisconnectHandler(global::Android.Widget.NumberPicker platformView)
        {
            platformView.ValueChanged -= OnValueChanged;
            base.DisconnectHandler(platformView);
        }

        private void OnValueChanged(object sender, global::Android.Widget.NumberPicker.ValueChangeEventArgs e)
        {
            if (VirtualView != null)
                VirtualView.Value = e.NewVal;
        }

        public static void MapMinimum(NumberPickerHandler handler, SleepTimer.Views.Controls.NumberPicker view) =>
            handler.PlatformView.MinValue = view.Minimum;

        public static void MapMaximum(NumberPickerHandler handler, SleepTimer.Views.Controls.NumberPicker view) =>
            handler.PlatformView.MaxValue = view.Maximum;

        public static void MapValue(NumberPickerHandler handler, SleepTimer.Views.Controls.NumberPicker view) =>
            handler.PlatformView.Value = view.Value;
    }
}
