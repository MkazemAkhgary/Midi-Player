using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    public sealed class LazySourceBindingBehavior : Behavior<Slider>
    {
        private Slider Host => AssociatedObject;
        private Thumb Thumb => SliderCompositeBehavior.GetThumb(Host);
        
        protected override void OnAttached()
        {
            SliderCompositeBehavior.SetSourceBindsToValue(Host, false);

            Host.Loaded += OnLoaded;
            Host.MouseLeftButtonUp += OnLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            SliderCompositeBehavior.SetSourceBindsToValue(Host, true);

            Host.Loaded -= OnLoaded;
            Host.MouseLeftButtonUp -= OnLeftButtonUp;
            Thumb.DragCompleted -= OnThumbDragCompleted;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragCompleted += OnThumbDragCompleted;
        }

        private void OnLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            SliderCompositeBehavior.SetSourceValue(Host, Host.Value);
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs args)
        {
            SliderCompositeBehavior.SetSourceValue(Host, Host.Value);
        }

    }
}
