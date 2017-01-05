using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.Slider
{
    using System.Windows.Controls;

    /// <summary>
    /// Hooked mouse event when dragging on slider.
    /// </summary>
    public sealed class FreeSlideBehavior : Behavior<Slider>
    {
        private DependencyPropertyProvider<Thumb, Slider> ThumbProperty { get; set; }
        
        private bool _isThumbGrabbed;

        private static Thumb InitializeThumb(Slider slider)
        {
            return ((Track)slider.Template.FindName("PART_Track", slider)).Thumb;
        }

        protected override void OnAttached()
        {
            ThumbProperty = DependencyPropertyProvider<Thumb, Slider>.Create(AssociatedObject, InitializeThumb);

            AssociatedObject.Loaded += ThumbProperty.Initialize;

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

                ThumbProperty.ProvidedProperty.RaiseEvent(mbArgs);
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
