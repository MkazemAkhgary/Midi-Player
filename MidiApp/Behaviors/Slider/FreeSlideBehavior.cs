using System.Windows;
using System.Windows.Input;

namespace MidiApp.Behaviors.Slider
{
    /// <summary>
    /// Provides easy sliding for the target slider.
    /// </summary>
    public sealed class FreeSlideBehavior : SliderBehavior
    {
        private bool _isThumbGrabbed;

        private readonly RoutedEventHandler _previewLeftButtonDown;
        private readonly RoutedEventHandler _previewLeftButtonUp;
        private readonly MouseEventHandler _previewMouseMove;

        public FreeSlideBehavior()
        {
            _previewLeftButtonDown = OnPreviewLeftButtonDown;
            _previewLeftButtonUp = OnPreviewLeftButtonUp;
            _previewMouseMove = OnPreviewMouseMove;
        }

        protected override void OnBehaviorAttached()
        {
            AssociatedObject.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, _previewLeftButtonDown, true);
            AssociatedObject.AddHandler(UIElement.PreviewMouseLeftButtonUpEvent, _previewLeftButtonUp, true);
            AssociatedObject.AddHandler(UIElement.PreviewMouseMoveEvent, _previewMouseMove, false);
        }

        protected override void OnBehaviorDetaching()
        {
            AssociatedObject.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, _previewLeftButtonDown);
            AssociatedObject.RemoveHandler(UIElement.PreviewMouseLeftButtonUpEvent, _previewLeftButtonUp);
            AssociatedObject.RemoveHandler(UIElement.PreviewMouseMoveEvent, _previewMouseMove);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs args)
        {
            // keep dragging when slider is clicked and mouse is moving.
            if (args.LeftButton == MouseButtonState.Pressed && _isThumbGrabbed)
            {
                var mbArgs = new MouseButtonEventArgs(args.MouseDevice, args.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = UIElement.MouseLeftButtonDownEvent,
                    Source = args.Source
                };

                Thumb.RaiseEvent(mbArgs);
            }
        }

        private void OnPreviewLeftButtonDown(object sender, RoutedEventArgs args)
        {
            _isThumbGrabbed = true;
        }

        private void OnPreviewLeftButtonUp(object sender, RoutedEventArgs args)
        {
            _isThumbGrabbed = false;
        }
    }
}
