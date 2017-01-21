using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    public class SmoothSlidingBehavior : Behavior<Slider>
    {
        private Track _track;

        private Slider Host => AssociatedObject;
        private Thumb Thumb => Track.Thumb;
        private Track Track => _track ?? (_track = SliderCompositeBehavior.GetTrack(Host));
        
        private readonly Storyboard _animator;
        private readonly DoubleAnimation _animation;
        
        private readonly Delegate _clickHandler;

        private double _oldVal;

        public SmoothSlidingBehavior()
        {
            _clickHandler = new MouseButtonEventHandler(OnLeftClick);

            _animation = new DoubleAnimation
            {
                AccelerationRatio = 0.1,
                DecelerationRatio = 0.9,
                SpeedRatio = 3
            };

            _animator = new Storyboard();
            _animator.Children.Add(_animation);
        }

        protected override void OnAttached()
        {
            Host.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, _clickHandler, true);
            
            Host.ValueChanged += OnValueChanged;

            Storyboard.SetTargetProperty(_animation, new PropertyPath("Value"));
            Storyboard.SetTarget(_animation, Host);
        }

        protected override void OnDetaching()
        {
            Host.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, _clickHandler);
            
            Host.ValueChanged -= OnValueChanged;

            _track = null;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            _oldVal = args.OldValue;
        }
        
        private void OnLeftClick(object sender, MouseButtonEventArgs args)
        {
            if(Thumb.IsMouseOver) return;
            UpdateTarget(_oldVal, Host.Value);
        }

        private void UpdateTarget(double oldVal, double newVal)
        {
            _animator.Stop();
            _animation.From = oldVal;
            _animation.To = newVal;
            _animator.Begin();
        }
    }
}