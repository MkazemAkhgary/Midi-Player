using System.Windows;
using System.Windows.Media.Animation;

namespace MidiApp.Animations
{
    class DoubleAnimationUsingDataBinding : DoubleAnimationBase
    {
        protected override Freezable CreateInstanceCore()
        {
            return new DoubleAnimationUsingDataBinding();
        }

        protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock)
        {
            return (double)animationClock.GetCurrentValue(defaultOriginValue, defaultDestinationValue);
        }
    }
}
