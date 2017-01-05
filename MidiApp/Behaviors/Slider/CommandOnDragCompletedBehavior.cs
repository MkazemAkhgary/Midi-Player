using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.Slider
{
    using System.Windows.Controls;

    /// <summary>
    /// Executes a command after slider drag completed.
    /// </summary>
    public sealed class CommandOnDragCompletedBehavior : Behavior<Slider>
    {
        private DependencyPropertyProvider<Thumb, Slider> ThumbProperty { get; set; }

        private static readonly DependencyProperty DragCompletedCommandProperty
            = DependencyProperty.Register(
                "DragCompletedCommand",
                typeof(ICommand),
                typeof(CommandOnDragCompletedBehavior));

        protected override void OnAttached()
        {
            ThumbProperty = DependencyPropertyProvider<Thumb, Slider>.Create(AssociatedObject, InitializeThumb);

            AssociatedObject.Loaded += ThumbProperty.Initialize;
        }

        private Thumb InitializeThumb(Slider slider)
        {
            var thumb = ((Track)slider.Template.FindName("PART_Track", slider)).Thumb;
            thumb.DragCompleted += OnThumbDragCompleted;
            return thumb;
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
