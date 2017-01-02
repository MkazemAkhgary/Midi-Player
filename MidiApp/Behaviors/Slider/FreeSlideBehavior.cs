using System.Windows;
using System.Windows.Input;

namespace MidiUI.Behaviors.Slider
{
    /// <summary>
    /// Hooked mouse event when dragging on slider.
    /// </summary>
    public sealed class FreeSlideBehavior : SliderBehavior
    {
        private bool _isThumbGrabbed;

        protected override void OnSliderAttached()
        {
            AssociatedObject.AddHandler(
                UIElement.PreviewMouseLeftButtonDownEvent, 
                new RoutedEventHandler(OnPreviewLeftButtonDown), 
                true);

            AssociatedObject.AddHandler(
                UIElement.PreviewMouseLeftButtonUpEvent,
                new RoutedEventHandler(OnPreviewLeftButtonUp), 
                true);

            AssociatedObject.AddHandler(
                UIElement.PreviewMouseMoveEvent, 
                new MouseEventHandler(OnPreviewMouseMove), 
                false);
        }

        private void OnPreviewLeftButtonDown(object sender, RoutedEventArgs args)
        {
            _isThumbGrabbed = true;
        }

        private void OnPreviewLeftButtonUp(object sender, RoutedEventArgs args)
        {
            _isThumbGrabbed = false;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs args)
        {
            // keep dragging when slider is clicked and mouse is moving.
            if (args.LeftButton == MouseButtonState.Pressed && _isThumbGrabbed)
            {
                Thumb.RaiseEvent(new MouseButtonEventArgs(args.MouseDevice, args.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = UIElement.MouseLeftButtonDownEvent,
                    Source = args.Source
                });
            }
        }
    }
}
