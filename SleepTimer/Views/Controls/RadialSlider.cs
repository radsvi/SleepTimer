using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace SleepTimer.Views.Controls
{
    public class RadialSlider : GraphicsView
    {
        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(double), typeof(RadialSlider), 0.0,
                propertyChanged: (b, o, n) => ((RadialSlider)b).Invalidate());

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create(nameof(Minimum), typeof(double), typeof(RadialSlider), 0.0);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create(nameof(Maximum), typeof(double), typeof(RadialSlider), 100.0);

        private bool _isDragging;
        private Point? _pointerStart; // absolute point where the pointer began (used with Pan fallback)

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

            // Ensure view can receive touches on platforms that ignore fully-transparent backgrounds
            BackgroundColor = Colors.Transparent;

            // Preferred: pointer events
            var pointer = new PointerGestureRecognizer();
            pointer.PointerPressed += OnPointerPressed;
            pointer.PointerMoved += OnPointerMoved;
            pointer.PointerReleased += OnPointerReleased;
            pointer.PointerExited += OnPointerExited;
            GestureRecognizers.Add(pointer);

            // Fallback: Pan (works reliably on many platforms)
            var pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(pan);
        }

        // ---------- Pointer handlers ----------
        private void OnPointerPressed(object? sender, PointerEventArgs e)
        {
            var p = e.GetPosition(this);
            Debug.WriteLine($"[RadialSlider] PointerPressed HasValue={p.HasValue}");
            if (!p.HasValue) return;

            _isDragging = true;
            _pointerStart = p.Value;
            UpdateValueFromPoint(p.Value);
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_isDragging) return;

            var p = e.GetPosition(this);
            if (!p.HasValue) return;

            UpdateValueFromPoint(p.Value);
        }

        private void OnPointerReleased(object? sender, PointerEventArgs e)
        {
            var p = e.GetPosition(this);
            Debug.WriteLine($"[RadialSlider] PointerReleased HasValue={p.HasValue}");
            if (p.HasValue) UpdateValueFromPoint(p.Value);

            _isDragging = false;
            _pointerStart = null;
        }

        private void OnPointerExited(object? sender, PointerEventArgs e)
        {
            Debug.WriteLine("[RadialSlider] PointerExited");
            _isDragging = false;
            _pointerStart = null;
        }

        // ---------- Pan fallback ----------
        private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            // If pointer start was captured, we can compute absolute position using TotalX/TotalY
            if (e.StatusType == GestureStatus.Running)
            {
                if (_pointerStart.HasValue)
                {
                    // absolute current point = startPoint + accumulated delta
                    var cur = new Point(_pointerStart.Value.X + e.TotalX, _pointerStart.Value.Y + e.TotalY);
                    UpdateValueFromPoint(cur);
                }
                else
                {
                    // fallback attempt: use center + total delta (less accurate but better than nothing)
                    var fallback = new Point(Width / 2 + e.TotalX, Height / 2 + e.TotalY);
                    UpdateValueFromPoint(fallback);
                }
            }
            else if (e.StatusType == GestureStatus.Completed || e.StatusType == GestureStatus.Canceled)
            {
                _isDragging = false;
                _pointerStart = null;
            }
            else if (e.StatusType == GestureStatus.Started)
            {
                Debug.WriteLine("[RadialSlider] Pan Started");
                // Nothing necessary here — pointer pressed usually already set _pointerStart.
            }
        }

        // ---------- conversion ----------
        private void UpdateValueFromPoint(Point touch)
        {
            var center = new Point(Width / 2, Height / 2);
            var dx = touch.X - center.X;
            var dy = touch.Y - center.Y;

            var angle = Math.Atan2(dy, dx) * 180.0 / Math.PI; // -180..180
            if (angle < 0) angle += 360.0;                     // 0..360

            var range = Maximum - Minimum;
            if (range <= 0) return;

            var newValue = Minimum + (angle / 360.0) * range;
            // clamp
            Value = Math.Max(Minimum, Math.Min(Maximum, newValue));
        }
    }

    // ---------- Drawable ----------
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
