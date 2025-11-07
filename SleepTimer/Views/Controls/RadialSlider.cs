using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;

namespace SleepTimer.Views.Controls
{
    public class RadialSlider : GraphicsView
    {
        public bool IsTouching { get; set; } = false;

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(double), typeof(RadialSlider), 0.0, BindingMode.TwoWay, propertyChanged: (b, o, n) => ((RadialSlider)b).Invalidate());

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create(nameof(Minimum), typeof(double), typeof(RadialSlider), 0.0);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create(nameof(Maximum), typeof(double), typeof(RadialSlider), 100.0);

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

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public RadialSlider()
        {
            Drawable = new RadialSliderDrawable(this);
            BackgroundColor = Colors.Transparent;

            StartInteraction += OnStartInteraction;
            DragInteraction += OnDragInteraction;
            EndInteraction += OnEndInteraction;
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
            var center = new Point(Width / 2, Height / 2);
            var dx = touch.X - center.X;
            var dy = touch.Y - center.Y;

            //var angle = Math.Atan2(dy, dx) * 180.0 / Math.PI; // -180..180
            var angle = Math.Atan2(dx, -dy) * 180.0 / Math.PI;
            if (angle < 0)
                angle += 360.0;

            var range = Maximum - Minimum;
            if (range <= 0)
                return;

            Value = Minimum + (angle / 360.0) * range;
        }
    }

    public class RadialSliderDrawable : IDrawable
    {
        private readonly RadialSlider _slider;
        public RadialSliderDrawable(RadialSlider slider) => _slider = slider;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float cx = (float)dirtyRect.Center.X;
            float cy = (float)dirtyRect.Center.Y;
            float r = Math.Min((float)dirtyRect.Width, (float)dirtyRect.Height) / 2 - 15;

            // Track
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 15;
            canvas.DrawCircle(cx, cy, r);

            // Progress arc
            double sweep = ((_slider.Value - _slider.Minimum) / (_slider.Maximum - _slider.Minimum)) * 360.0;
            canvas.StrokeColor = Colors.DodgerBlue;
            canvas.StrokeSize = 12;
            canvas.DrawArc(cx - r, cy - r, r * 2, r * 2, -270F, -(float)sweep -270F, true, false);

            // Thumb
            double rad = (sweep - 90.0) * Math.PI / 180.0;
            float tx = cx + (float)(r * Math.Cos(rad));
            float ty = cy + (float)(r * Math.Sin(rad));
            canvas.FillColor = Colors.DodgerBlue;
            canvas.FillCircle(tx, ty, 15);
            // Thumb on touching
            if (_slider.IsTouching)
            {
                canvas.FillColor = Colors.DodgerBlue.WithAlpha(0.5f);
                canvas.FillCircle(tx, ty, 25);
            }

            // Value
            canvas.FontColor = Colors.LightGray;
            //canvas.FontSize = 30;
            canvas.FontSize = (Math.Min(dirtyRect.Width, dirtyRect.Height) / 4);
            canvas.DrawString(_slider.Value.ToString("N0"), dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.FontSize = (Math.Min(dirtyRect.Width, dirtyRect.Height) / 12);
            var lowerRect = new Rect(
                dirtyRect.X,
                dirtyRect.Y + dirtyRect.Height / 2 + 30,
                dirtyRect.Width,
                dirtyRect.Height
            );
            canvas.DrawString("MINUTES", lowerRect, HorizontalAlignment.Center, VerticalAlignment.Top);

        }
    }
}
