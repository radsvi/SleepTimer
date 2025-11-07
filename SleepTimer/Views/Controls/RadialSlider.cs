using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;

namespace SleepTimer.Views.Controls
{
    public class RadialSlider : GraphicsView
    {
        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(double), typeof(RadialSlider), 0.0, propertyChanged: (b, o, n) => ((RadialSlider)b).Invalidate());

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
            UpdateValueFromPoint(e.Touches[0]);
        }

        private void OnDragInteraction(object? sender, TouchEventArgs e)
        {
            UpdateValueFromPoint(e.Touches[0]);
        }

        private void OnEndInteraction(object? sender, TouchEventArgs e)
        {
            UpdateValueFromPoint(e.Touches[0]);
        }

        private void UpdateValueFromPoint(Point touch)
        {
            var center = new Point(Width / 2, Height / 2);
            var dx = touch.X - center.X;
            var dy = touch.Y - center.Y;

            var angle = Math.Atan2(dy, dx) * 180.0 / Math.PI; // -180..180
            if (angle < 0) angle += 360.0;

            var range = Maximum - Minimum;
            if (range <= 0) return;

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
            //canvas.StrokeColor = Colors.DodgerBlue;
            //canvas.StrokeSize = 12;
            //canvas.DrawArc(cx - r, cy - r, r * 2, r * 2, -90, (float)sweep, false, false);

            // Thumb
            double rad = (sweep) * Math.PI / 180.0;
            float tx = cx + (float)(r * Math.Cos(rad));
            float ty = cy + (float)(r * Math.Sin(rad));
            canvas.FillColor = Colors.Red;
            canvas.FillCircle(tx, ty, 15);

            // Value
            var attrs1 = new TextAttributes();
            attrs1.SetFontSize(24f);           // extension method
            attrs1.SetAttribute(TextAttribute.FontName, "Arial");
            attrs1.SetForegroundColor("#ef0e0e");
            var run1 = new AttributedTextRun(0, 7, attrs1);

            var runs = new List<IAttributedTextRun> { run1 };


            canvas.FontColor = Colors.White;

            var text = "Hello";
            var attributed = new AttributedText(text, runs);
            //canvas.DrawText(attributed, 20, 20, 20, 20);
            canvas.DrawString("Hello", new Rect(0, 0, 40, 40), HorizontalAlignment.Center, VerticalAlignment.Center);

        }
    }
}
