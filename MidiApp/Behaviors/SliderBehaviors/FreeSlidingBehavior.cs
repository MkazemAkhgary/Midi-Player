using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// when IsMoveToPointEnabled is true user can grab thumb anywhere from slider and drag slider.
    /// </summary>
    public sealed class FreeSlidingBehavior : Behavior<Slider>
    {
        private Thumb _thumb;

        private Slider Host => AssociatedObject;
        private Thumb Thumb => _thumb ?? (_thumb = SliderCompositeBehavior.GetTrack(Host).Thumb);

        private readonly Delegate _clickHandler;
        private bool _clicked;
        private bool _handled;

        public FreeSlidingBehavior()
        {
            _clickHandler = new MouseButtonEventHandler((o, e) => _clicked = true);
        }

        protected override void OnAttached()
        {
            Host.Loaded += OnLoaded;
            Host.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, _clickHandler, true);
            Host.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            Host.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, _clickHandler);
            Thumb.DragCompleted -= OnDragCompleted;
            Host.MouseMove -= OnMouseMove;

            _thumb = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragCompleted += OnDragCompleted;
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs args)
        {
            _clicked = false;
            _handled = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (_handled || !_clicked || !Host.IsMouseOver)
                return;

            _handled = true;

            Thumb.RaiseEvent(new MouseButtonEventArgs(args.MouseDevice, args.Timestamp, MouseButton.Left)
            {
                RoutedEvent = UIElement.MouseLeftButtonDownEvent
            });
        }
    }
}
