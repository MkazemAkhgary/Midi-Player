using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// prevents updating value from source when thumb is being dragged.
    /// source value is updated when dragging is completed.
    /// </summary>
    public sealed class LockOnDragBehavior : Behavior<Slider>
    {
        private Slider Host => AssociatedObject;
        private Thumb Thumb => SliderCompositeBehavior.GetThumb(Host);

        protected override void OnAttached()
        {
            Host.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            Host.Loaded -= OnLoaded;

            Thumb.DragStarted -= OnDragStarted;
            Thumb.DragCompleted -= OnDragCompleted;

            SliderCompositeBehavior.BindValueWithSource(Host, true);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragStarted += OnDragStarted;
            Thumb.DragCompleted += OnDragCompleted;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs args)
        {
            SliderCompositeBehavior.BindValueWithSource(Host, false);
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs args)
        {
            SliderCompositeBehavior.BindValueWithSource(Host, true);
        }
    }
}
