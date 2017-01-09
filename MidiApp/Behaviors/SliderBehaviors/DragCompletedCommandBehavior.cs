using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// Provides binding for a command that executes after dragging slider is completed.
    /// </summary>
    public sealed class DragCompletedCommandBehavior : Behavior<Slider>
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
            Thumb.DragCompleted -= OnThumbDragCompleted;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragCompleted += OnThumbDragCompleted;
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs args)
        {
            if (Command?.CanExecute(Host.Value) ?? false)
            {
                Command?.Execute(Host.Value);
            }
        }
        
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(DragCompletedCommandBehavior));
    }
}
