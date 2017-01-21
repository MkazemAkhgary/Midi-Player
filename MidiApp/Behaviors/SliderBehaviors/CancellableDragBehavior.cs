using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// Cancels the sliding operation on right Click.
    /// </summary>
    public class CancellableDragBehavior : Behavior<Slider>
    {
        private bool _bindsDefault;
        
        private Thumb _thumb;

        private Slider Host => AssociatedObject;
        private Thumb Thumb => _thumb ?? (_thumb = SliderCompositeBehavior.GetTrack(Host).Thumb);

        protected override void OnAttached()
        {
            _bindsDefault = SliderCompositeBehavior.GetSourceBindsToValue(Host);
            if (_bindsDefault) SliderCompositeBehavior.SetSourceBindsToValue(Host, false);

            Host.MouseLeftButtonUp += OnLeftButtonUp;
            Host.MouseRightButtonUp += OnRightButtonUp;
            Host.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            if (_bindsDefault) SliderCompositeBehavior.SetSourceBindsToValue(Host, true);
            
            Host.MouseLeftButtonUp -= OnLeftButtonUp;
            Host.MouseRightButtonUp -= OnRightButtonUp;
            Thumb.DragCompleted -= OnThumbDragCompleted;
            Host.Loaded -= OnLoaded;

            _thumb = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Thumb.DragCompleted += OnThumbDragCompleted;
        }

        private void OnLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            SliderCompositeBehavior.SetSourceValue(Host, Host.Value);
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs args)
        {
            SliderCompositeBehavior.SetSourceValue(Host, Host.Value);
        }

        private void OnRightButtonUp(object sender, MouseButtonEventArgs args)
        {
            if(Thumb.IsDragging)
            {
                Host.Value = SliderCompositeBehavior.GetSourceValue(Host);
                Thumb.CancelDrag();
            }
        }
    }
}
