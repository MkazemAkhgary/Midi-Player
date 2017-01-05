using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MidiApp.Behaviors.Slider
{
    /// <summary>
    /// Provides binding for a command that executes after dragging slider is completed.
    /// </summary>
    public sealed class DragCompletedCommandBehavior : SliderBehavior
    {
        protected override void OnBehaviorAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectLoaded;
        }

        protected override void OnBehaviorDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Thumb.DragCompleted += OnThumbDragCompleted;
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            if (Command.CanExecute(Value))
            {
                Command.Execute(Value);
            }
        }

        public new ICommand Command
        {
            get { return base.Command; }
            set { base.Command = value; }
        }
    }
}
