using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;
// ReSharper disable MemberCanBePrivate.Global

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
            Host.MouseLeftButtonUp += OnLeftButtonUp;
        }

        protected override void OnDetaching()
        {
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
            RaiseCommand(Host.Value);
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs args)
        {
            RaiseCommand(Host.Value);
        }

        private void RaiseCommand(double value)
        {
            if (Command?.CanExecute(value) ?? false) Command.Execute(value);
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
