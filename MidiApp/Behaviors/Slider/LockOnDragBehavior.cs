using System.Windows;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors.Slider
{
    /// <summary>
    /// provides seperate binding for slider. 
    /// prevents updating value from source when thumb is being dragged.
    /// source value is updated when dragging is completed.
    /// </summary>
    public sealed class LockOnDragBehavior : SliderBehavior
    {
        private static readonly DependencyProperty SourceValueProperty =
            DependencyProperty.Register(
                "SourceValue", typeof(double), typeof(LockOnDragBehavior),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnSourceValueChanged)
                ));

        public double SourceValue
        {
            get { return (double)GetValue(SourceValueProperty); }
            set { SetValue(SourceValueProperty, value); }
        }

        private bool IsFree => !Thumb.IsDragging;

        protected override void OnBehaviorAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectLoaded;
            AssociatedObject.ValueChanged += OnValueChanged;
        }

        protected override void OnBehaviorDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
            AssociatedObject.ValueChanged -= OnValueChanged;
        }

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Thumb.DragCompleted += OnThumbDragCompleted;
        }
        
        private static void OnSourceValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var owner = (LockOnDragBehavior)dependencyObject;

            // update slider value if thumb is not grabbed.
            if (owner.IsFree)
            {
                var slider = owner.AssociatedObject;
                slider.SetValue(
                        RangeBase.ValueProperty,
                        dependencyPropertyChangedEventArgs.NewValue
                    );
            }
        }
        
        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs)
        {
            // update slider value if thumb is not grabbed. the value is changing from extenrnal code.
            if (IsFree)
            {
                SourceValue = Value;
            }
        }

        // update source after dragging compeleted.
        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            SourceValue = Value;
        }
    }
}
