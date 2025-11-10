using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;

namespace SleepTimer.Views.Controls
{
    public class RadialSlider : GraphicsView
    {
        private double _cumulativeAngle;
        //private double _lastAngle = 0;

        private bool _isValuePropertyInitialized;

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(double), typeof(RadialSlider), 0.0, BindingMode.TwoWay, propertyChanged: (b, o, n) => ((RadialSlider)b).Invalidate());

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create(nameof(Minimum), typeof(double), typeof(RadialSlider), 0.0);

        public static readonly BindableProperty FullTurnValueProperty =
            BindableProperty.Create(nameof(FullTurnValue), typeof(double), typeof(RadialSlider), 60.0);

        public static readonly BindableProperty UnitsProperty =
            BindableProperty.Create(nameof(Units), typeof(string), typeof(RadialSlider), string.Empty);

        public static readonly BindableProperty TouchControlProperty =
            BindableProperty.Create(nameof(TouchControl), typeof(bool), typeof(RadialSlider), true, propertyChanged: (b, o, n) => ((RadialSlider)b).TouchControlChanged());

        public static readonly BindableProperty ValueRemainingProperty =
            BindableProperty.Create(nameof(ValueRemaining), typeof(TimeSpan), typeof(RadialSlider), TimeSpan.Zero, propertyChanged: (b, o, n) => ((RadialSlider)b).ValueRemainingChanged());


        public bool IsTouching { get; set; } = false;
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double FullTurnValue
        {
            get => (double)GetValue(FullTurnValueProperty);
            set => SetValue(FullTurnValueProperty, value);
        }
        public string Units
        {
            get => (string)GetValue(UnitsProperty);
            set => SetValue(UnitsProperty, value);
        }
        public bool TouchControl
        {
            get { return (bool)GetValue(TouchControlProperty); }
            set { SetValue(TouchControlProperty, value); }
        }
        public TimeSpan ValueRemaining
        {
            get { return (TimeSpan)GetValue(ValueRemainingProperty); }
            set { SetValue(ValueRemainingProperty, value); }
        }

        public RadialSlider()
        {
            Drawable = new RadialSliderDrawable(this);
            BackgroundColor = Colors.Transparent;

            if (TouchControl)
                EnableControl();

            Loaded += OnLoaded;
        }
        private void TouchControlChanged()
        {
            if (TouchControl)
                EnableControl();
            else
                DisableControl();

            System.Diagnostics.Debug.WriteLine($"## Changing TouchControl: {TouchControl}");
            Invalidate();
        }
        private void EnableControl()
        {
            StartInteraction += OnStartInteraction;
            DragInteraction += OnDragInteraction;
            EndInteraction += OnEndInteraction;
        }
        private void DisableControl()
        {
            StartInteraction -= OnStartInteraction;
            DragInteraction -= OnDragInteraction;
            EndInteraction -= OnEndInteraction;
        }
        private void ValueRemainingChanged()
        {
            System.Diagnostics.Debug.WriteLine($"## Updating canvas. ValueRemaining: {ValueRemaining}");
            Invalidate();
        }

        private void OnLoaded(object? sender, EventArgs e)
        {
            if (_isValuePropertyInitialized)
                return;

            _isValuePropertyInitialized = true;
            OnAllPropertiesInitialized();
        }

        private void OnAllPropertiesInitialized()
        {
            _cumulativeAngle = ((Value - Minimum) / (FullTurnValue - Minimum)) * 360.0;
        }

        private void OnStartInteraction(object? sender, TouchEventArgs e)
        {
            IsTouching = true;
            UpdateValueFromPoint(e.Touches[0]);
            Invalidate(); // force redraw
        }

        private void OnDragInteraction(object? sender, TouchEventArgs e)
        {
            UpdateValueFromPoint(e.Touches[0]);
            Invalidate();
        }

        private void OnEndInteraction(object? sender, TouchEventArgs e)
        {
            IsTouching = false;
            UpdateValueFromPoint(e.Touches[0]);
            Invalidate();
        }

        private void UpdateValueFromPoint(Point touch)
        {
            if (FullTurnValue <= Minimum)
                return;

            var center = new Point(Width / 2, Height / 2);
            var dx = touch.X - center.X;
            var dy = touch.Y - center.Y;

            var angle = Math.Atan2(dx, -dy) * 180.0 / Math.PI;
            if (angle < 0) angle += 360.0;

            // Detect wrap-around
            var lastAngle = _cumulativeAngle % 360;
            double delta = angle - lastAngle;
            if (delta < -180) delta += 360; // wrapped past 0
            else if (delta > 180) delta -= 360; // wrapped past 360

            var newCumulativeAngle = _cumulativeAngle + delta;
            if (newCumulativeAngle < 0)
                _cumulativeAngle = 0;
            else
                _cumulativeAngle = newCumulativeAngle;

            Value = Minimum + (_cumulativeAngle / 360.0) * (FullTurnValue - Minimum);

            //System.Diagnostics.Debug.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] Value:{Value:N0} | _cumulativeAngle:{_cumulativeAngle:N0} | _lastAngle:{lastAngle:N0} | angle:{angle:N0} | delta:{delta:N0}");
        }
    }

    public class RadialSliderDrawable : IDrawable
    {
        const float Scale = 0.8f;

        private readonly RadialSlider _slider;
        public RadialSliderDrawable(RadialSlider slider) => _slider = slider;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            System.Diagnostics.Debug.WriteLine($"### Draving new canvas | _slider.TouchControl {_slider.TouchControl} | _slider.ValueRemaining {_slider.ValueRemaining}");
            if (_slider.TouchControl)
            {
                DrawGraphic(canvas, dirtyRect);
                DrawText(canvas, dirtyRect, new DrawTextFields(_slider.Value, _slider.Units));
            }
            else
            {
                DrawText(canvas, dirtyRect, new DrawTextFields(_slider.ValueRemaining, _slider.Units + " remaining"));
            }
        }


        private void DrawGraphic(ICanvas canvas, RectF dirtyRect)
        {
            float cx = (float)dirtyRect.Center.X;
            float cy = (float)dirtyRect.Center.Y;
            float r = Scale * Math.Min((float)dirtyRect.Width, (float)dirtyRect.Height) / 2 - 15;

            // Track
            canvas.StrokeColor = Color.FromArgb("424242");
            canvas.StrokeSize = 12;
            canvas.DrawCircle(cx, cy, r);

            // Progress arc
            double sweep = ((_slider.Value - _slider.Minimum) / (_slider.FullTurnValue - _slider.Minimum)) * 360.0;
            canvas.StrokeColor = Colors.DodgerBlue;
            canvas.StrokeSize = 12;
            if (_slider.Value >= _slider.FullTurnValue)
                canvas.DrawCircle(cx, cy, r);
            else
                canvas.DrawArc(cx - r, cy - r, r * 2, r * 2, -270F, -(float)sweep - 270F, true, false);

            // Thumb
            double rad = (sweep - 90.0) * Math.PI / 180.0;
            float tx = cx + (float)(r * Math.Cos(rad));
            float ty = cy + (float)(r * Math.Sin(rad));
            //canvas.FillColor = Colors.DodgerBlue;
            canvas.FillColor = Color.FromArgb("0984FF");
            canvas.FillCircle(tx, ty, 15);

            // Thumb on touching
            if (_slider.IsTouching)
            {
                canvas.FillColor = Colors.DodgerBlue.WithAlpha(0.5f);
                canvas.FillCircle(tx, ty, 25);
            }
        }
        private void DrawText(ICanvas canvas, RectF dirtyRect, DrawTextFields textFields)
        {
            // Text value
            canvas.FontColor = Color.FromArgb("676767");
            //canvas.FontSize = 30;

            canvas.FontSize = Scale * (Math.Min(dirtyRect.Width, dirtyRect.Height) / 2.2F);
            canvas.Font = new Microsoft.Maui.Graphics.Font("sans-serif-condensed");
            canvas.DrawString(textFields.TextValue, (RectF)dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.FontSize = Scale * (Math.Min(dirtyRect.Width, dirtyRect.Height) / 14);
            var lowerRect = new Rect(
                dirtyRect.X,
                (int)(dirtyRect.Y + dirtyRect.Height / 2 + 56),
                dirtyRect.Width,
                dirtyRect.Height
            );
            canvas.DrawString(textFields.SubText.ToUpper(), lowerRect, HorizontalAlignment.Center, VerticalAlignment.Top);
        }
    }
}
