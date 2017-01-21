using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// Cancels the sliding operation on right Click.
    /// </summary>
    public class CancellableDragBehavior : Behavior<Slider>
    {
        private Thumb _thumb;

        private Slider Host => AssociatedObject;
        private Thumb Thumb => _thumb ?? (_thumb = SliderCompositeBehavior.GetTrack(Host).Thumb);

        private double _oldVal;

        protected override void OnAttached()
        {
            Host.MouseRightButtonUp += OnRightButtonUp;
            Host.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            Host.MouseRightButtonUp -= OnRightButtonUp;
            Thumb.DragStarted -= OnDragStarted;
            Host.Loaded -= OnLoaded;

            _thumb = null;
            _oldVal = 0d;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragStarted += OnDragStarted;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs args)
        {
            _oldVal = Host.Value;
        }

        private void OnRightButtonUp(object sender, MouseButtonEventArgs args)
        {
            if(Thumb.IsDragging)
            {
                Host.Value = SliderCompositeBehavior.GetSourceBindsToValue(Host)
                ? _oldVal
                : SliderCompositeBehavior.GetSourceValue(Host);
                Thumb.CancelDrag();
            }
        }
    }
}
