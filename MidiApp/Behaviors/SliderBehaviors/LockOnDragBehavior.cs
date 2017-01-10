using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// prevents updating value by source when thumb is being dragged.
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
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragStarted += OnDragStarted;
            Thumb.DragCompleted += OnDragCompleted;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs args)
        {
            SliderCompositeBehavior.SetValueBindsToSource(Host, false);
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs args)
        {
            SliderCompositeBehavior.SetValueBindsToSource(Host, true);
        }
    }
}
