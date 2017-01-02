using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MidiUI.Behaviors.Slider
{
    /// <summary>
    /// Executes a command after slider drag completed.
    /// </summary>
    public sealed class CommandOnDragCompletedBehavior : SliderBehavior
    {
        private static readonly DependencyProperty DragCompletedCommandProperty
            = DependencyProperty.Register(
                "DragCompletedCommand",
                typeof(ICommand),
                typeof(CommandOnDragCompletedBehavior));

        protected override void OnSliderAttached()
        {
        }

        protected override void OnSliderLoaded()
        {
            Thumb.DragCompleted += OnThumbDragCompleted;
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            if (DragCompletedCommand.CanExecute(AssociatedObject.Value))
            {
                DragCompletedCommand.Execute(AssociatedObject.Value);
            }
        }

        public ICommand DragCompletedCommand
        {
            get { return (ICommand) GetValue(DragCompletedCommandProperty); }
            set { SetValue(DragCompletedCommandProperty, value); }
        }
    }
}
