using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.Slider
{
    using System.Windows.Controls;

    /// <summary>
    /// provides seperate binding for slider. 
    /// prevents updating value from source when thumb is being dragged.
    /// source value is updated when dragging is completed.
    /// </summary>
    public sealed class LockOnDragBehavior : Behavior<Slider>
    {
        private DependencyPropertyProvider<Thumb, Slider> ThumbProperty { get; set; }

        public double SourceValue
        {
            get { return (double)GetValue(SourceValueProperty); }
            set { SetValue(SourceValueProperty, value); }
        }

        private static readonly DependencyProperty SourceValueProperty =
            DependencyProperty.Register(
                "SourceValue", typeof(double), typeof(LockOnDragBehavior),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnSourceValueChanged)
                ));

        private bool IsFree => !ThumbProperty.ProvidedProperty.IsDragging;

        protected override void OnAttached()
        {
            ThumbProperty = DependencyPropertyProvider<Thumb, Slider>.Create(AssociatedObject, InitializeThumb);

            AssociatedObject.Loaded += ThumbProperty.Initialize;

            AssociatedObject.ValueChanged += AssociatedObjectOnValueChanged;
        }

        private Thumb InitializeThumb(Slider slider)
        {
            var thumb = ((Track)slider.Template.FindName("PART_Track", slider)).Thumb;
            thumb.DragCompleted += OnThumbDragCompleted;
            return thumb;
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

        // update source after dragging compeleted.
        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            SourceValue = AssociatedObject.Value;
        }

        private void AssociatedObjectOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs)
        {
            // update slider value if thumb is not grabbed. the value is changing from extenrnal code.
            if (IsFree)
            {
                SourceValue = AssociatedObject.Value;
            }
        }
    }
}
