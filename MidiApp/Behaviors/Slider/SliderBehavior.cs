using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.Slider
{
    using System.Windows.Controls;

    /// <summary>
    /// Provides useful properties for slider behaviors.
    /// </summary>
    public abstract class SliderBehavior : Behavior<Slider>
    {
        private static readonly DependencyProperty ThumbProperty =
            DependencyProperty.Register(
                "Thumb", 
                typeof(Thumb),
                typeof(SliderBehavior));

        private static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(SliderBehavior));

        protected double Maximum
        {
            get { return (double) AssociatedObject.GetValue(RangeBase.MaximumProperty); }
            set { AssociatedObject.SetValue(RangeBase.MaximumProperty, value); }
        }

        protected double Minimum
        {
            get { return (double)AssociatedObject.GetValue(RangeBase.MinimumProperty); }
            set { AssociatedObject.SetValue(RangeBase.MinimumProperty, value); }
        }

        protected double Value
        {
            get { return (double)AssociatedObject.GetValue(RangeBase.ValueProperty); }
            set { AssociatedObject.SetValue(RangeBase.ValueProperty, value); }
        }

        protected Thumb Thumb
        {
            get { return (Thumb)AssociatedObject.GetValue(ThumbProperty); }
            set { AssociatedObject.SetValue(ThumbProperty, value); }
        }

        protected ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected sealed override void OnAttached()
        {
            AssociatedObject.Loaded += InitializeThumb;
            OnBehaviorAttached();
        }

        protected sealed override void OnDetaching()
        {
            AssociatedObject.Loaded -= InitializeThumb;
            OnBehaviorDetaching();
        }

        protected virtual void OnBehaviorAttached()
        {
        }

        protected virtual void OnBehaviorDetaching()
        {
        }

        private void InitializeThumb(object sender, RoutedEventArgs routedEventArgs)
        {
            Thumb = ((Track) AssociatedObject.Template.FindName("PART_Track", AssociatedObject)).Thumb;
        }
    }
}
