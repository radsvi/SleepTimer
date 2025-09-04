using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

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

        private bool _isDragging;

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

            // Optional: tap to jump the thumb
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => UpdateValueFromPoint((Point)e.GetPosition(this));
            GestureRecognizers.Add(tap);

            //// Pointer (mouse/touch/pen) drag tracking
            //var pointer = new PointerGestureRecognizer();
            //pointer.PointerPressed += (s, e) =>
            //{
            //    _isDragging = true;
            //    UpdateValueFromPoint((Point)e.GetPosition(this));
            //};
            //pointer.PointerMoved += (s, e) =>
            //{
            //    if (_isDragging)
            //        UpdateValueFromPoint((Point)e.GetPosition(this));
            //};
            //pointer.PointerReleased += (s, e) =>
            //{
            //    UpdateValueFromPoint((Point)e.GetPosition(this));
            //    _isDragging = false;
            //};
            //pointer.PointerReleased += (s, e) => _isDragging = false; // safety

            var pointer = new PointerGestureRecognizer();
            pointer.PointerPressed += OnPointerPressed;
            pointer.PointerMoved += OnPointerMoved;
            pointer.PointerReleased += OnPointerReleased;
            pointer.PointerExited += OnPointerExited; // use this instead of PointerCanceled

            GestureRecognizers.Add(pointer);
        }
        private void OnPointerPressed(object? sender, PointerEventArgs e)
        {
            _isDragging = true;
            UpdateFromPointerEvent(e);
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging)
                UpdateFromPointerEvent(e);
        }

        private void OnPointerReleased(object? sender, PointerEventArgs e)
        {
            UpdateFromPointerEvent(e);
            _isDragging = false;
        }

        private void OnPointerExited(object? sender, PointerEventArgs e)
        {
            // pointer left the view — cancel dragging
            _isDragging = false;
        }
        private void UpdateFromPointerEvent(PointerEventArgs e)
        {
            // GetPosition returns Point? — check for null
            var p = e.GetPosition(this);
            if (!p.HasValue) return;

            UpdateValueFromPoint(p.Value); // your existing method that converts the point -> angle -> Value
        }

        private void UpdateValueFromPoint(Point touch)
        {
            var center = new Point(Width / 2, Height / 2);
            var dx = touch.X - center.X;
            var dy = touch.Y - center.Y;

            var angle = Math.Atan2(dy, dx) * 180 / Math.PI; // -180..180
            if (angle < 0) angle += 360;                    // 0..360

            var range = Maximum - Minimum;
            if (range <= 0) return;

            Value = Minimum + (angle / 360.0) * range;
        }
    }

    public class RadialSliderDrawable : IDrawable
    {
        private readonly RadialSlider _slider;
        public RadialSliderDrawable(RadialSlider slider) => _slider = slider;

        public void Draw(ICanvas canvas, RectF dirty)
        {
            float cx = (float)dirty.Center.X;
            float cy = (float)dirty.Center.Y;
            float r = Math.Min((float)dirty.Width, (float)dirty.Height) / 2 - 15;

            // Track
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 10;
            canvas.DrawCircle(cx, cy, r);

            // Progress arc
            double sweep = ((_slider.Value - _slider.Minimum) / (_slider.Maximum - _slider.Minimum)) * 360.0;
            canvas.StrokeColor = Colors.DodgerBlue;
            canvas.StrokeSize = 12;
            canvas.DrawArc(cx - r, cy - r, r * 2, r * 2, -90, (float)sweep, false, false);

            // Thumb
            double rad = (sweep - 90) * Math.PI / 180.0;
            float tx = cx + (float)(r * Math.Cos(rad));
            float ty = cy + (float)(r * Math.Sin(rad));
            canvas.FillColor = Colors.Red;
            canvas.FillCircle(tx, ty, 15);
        }
    }
}
