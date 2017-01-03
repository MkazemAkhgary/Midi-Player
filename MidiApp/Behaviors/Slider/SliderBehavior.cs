using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.Slider
{
    /// <summary>
    /// provides base class for silder behaviors.
    /// </summary>
    public abstract class SliderBehavior : Behavior<System.Windows.Controls.Slider>
    {
        protected static readonly DependencyProperty ThumbProperty =
            DependencyProperty.Register("Thumb", typeof(Thumb), typeof(SliderBehavior));

        protected Thumb Thumb
        {
            get { return (Thumb)GetValue(ThumbProperty); }
            private set { SetValue(ThumbProperty, value); }
        }

        protected sealed override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            OnSliderAttached();
        }
        
        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Thumb = ((Track) AssociatedObject.Template.FindName("PART_Track", AssociatedObject)).Thumb;
            OnSliderLoaded();
        }

        protected abstract void OnSliderAttached();

        protected virtual void OnSliderLoaded()
        {
        }
    }
}
