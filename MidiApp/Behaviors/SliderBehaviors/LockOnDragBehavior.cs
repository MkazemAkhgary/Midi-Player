using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// provides secondary binding for slider (SourceValue).
    /// prevents updating value from source when thumb is being dragged.
    /// source value is updated when dragging is completed.
    /// </summary>
    public sealed class LockOnDragBehavior : Behavior<Slider>
    {
        private Slider Host => AssociatedObject;
        private Thumb Thumb => SliderCompositeBehavior.GetThumb(Host);

        static LockOnDragBehavior()
        {
            SliderCompositeBehavior.SourceValueProperty.OverrideMetadata(
                typeof(LockOnDragBehavior),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSourceValueChanged));
        }

        protected override void OnAttached()
        {
            SliderCompositeBehavior.MaximumPropertyDescriptor.AddValueChanged(Host, OnMaximumChanged);
            SliderCompositeBehavior.MinimumPropertyDescriptor.AddValueChanged(Host, OnMinimumChanged);

            Host.Loaded += OnLoaded;

            SliderCompositeBehavior.SetBindValueToSource(Host, false);
        }

        protected override void OnDetaching()
        {
            SliderCompositeBehavior.MaximumPropertyDescriptor.RemoveValueChanged(Host, OnMaximumChanged);
            SliderCompositeBehavior.MinimumPropertyDescriptor.RemoveValueChanged(Host, OnMinimumChanged);

            Host.Loaded -= OnLoaded;

            Thumb.DragCompleted -= OnDragCompleted;

            SliderCompositeBehavior.SetBindValueToSource(Host, true);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragCompleted += OnDragCompleted;
        }

        private void OnMaximumChanged(object sender, EventArgs args)
        {
            SliderCompositeBehavior.SetSourceValue(Host, Host.Value);
        }

        private void OnMinimumChanged(object sender, EventArgs args)
        {
            SliderCompositeBehavior.SetSourceValue(Host, Host.Value);
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs args)
        {
            SliderCompositeBehavior.SetSourceValue(Host, Host.Value);
        }

        private static void OnSourceValueChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var soruce = (LockOnDragBehavior)dpo;
            if (!soruce.Thumb.IsDragging)
            {
                soruce.Host.Value = (double) args.NewValue; 
            }
        }
    }
}
