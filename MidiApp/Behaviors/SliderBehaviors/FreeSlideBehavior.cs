﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// when IsMoveToPointEnabled is true user can grab thumb anywhere from slider and drag.
    /// </summary>
    public sealed class FreeSlideBehavior : Behavior<Slider>
    {
        private Track _track;

        private Slider Host => AssociatedObject;
        private Thumb Thumb => Track.Thumb;
        private Track Track => _track ?? (_track = SliderCompositeBehavior.GetTrack(Host));

        protected override void OnAttached()
        {
            Host.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            Host.MouseMove -= OnMouseMove;

            _track = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (Thumb.IsDragging) return;
            if (!Thumb.IsMouseOver) return;
            if (args.LeftButton == MouseButtonState.Released) return;

            Thumb.RaiseEvent(new MouseButtonEventArgs(args.MouseDevice, args.Timestamp, MouseButton.Left)
            {
                RoutedEvent = UIElement.MouseLeftButtonDownEvent
            });
        }
    }
}
