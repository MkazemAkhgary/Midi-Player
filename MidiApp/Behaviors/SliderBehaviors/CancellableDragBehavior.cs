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
        private Slider Host => AssociatedObject;
        private Thumb Thumb => SliderCompositeBehavior.GetThumb(Host);

        protected override void OnAttached()
        {
            Host.MouseRightButtonUp += OnMouseRightButtonUp;
        }

        protected override void OnDetaching()
        {
            Host.MouseRightButtonUp -= OnMouseRightButtonUp;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs args)
        {
            Host.Value = SliderCompositeBehavior.GetSourceValue(Host);
            Thumb.CancelDrag();
        }
    }
}
