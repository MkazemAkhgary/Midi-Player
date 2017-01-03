using System.Windows.Controls;

namespace MidiApp.Controls
{
    /// <summary>
    /// Slider that corrects ratio of it's value when ever it's boundary changes.
    /// </summary>
    public class RatioSlider : Slider
    {
        private bool _selfChanging;
        private double _oldValue;

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            lock (this)
            {
                if (_selfChanging) return;
                _selfChanging = true;

                if (newMaximum < Minimum) // not tested. 
                {
                    IsDirectionReversed = !IsDirectionReversed;

                    Maximum = Minimum;
                    newMaximum = Maximum;
                    Minimum = newMaximum;
                }

                if ((oldMaximum - Minimum) <= 0) oldMaximum = Minimum + 1; // prevent division by zero or negative result.

                if (newMaximum > Value) // use original value if slider did not touch Value, (i.e when Value = newMax)
                    _oldValue = Value;

                Value = _oldValue * (newMaximum - Minimum) / (oldMaximum - Minimum);

                base.OnMaximumChanged(oldMaximum, newMaximum);

                _selfChanging = false;
            }
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            lock (this)
            {
                if (_selfChanging) return;
                _selfChanging = true;

                if (newMinimum > Maximum)
                {
                    IsDirectionReversed = !IsDirectionReversed;

                    Minimum = Maximum;
                    newMinimum = Minimum;
                    Maximum = newMinimum;
                }

                if ((Maximum - oldMinimum) <= 0) oldMinimum = Maximum - 1;

                if (newMinimum < Value)
                    _oldValue = Value;

                Value = _oldValue * (Maximum - newMinimum)/(Maximum - oldMinimum);

                base.OnMinimumChanged(oldMinimum, newMinimum);

                _selfChanging = false;
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            _oldValue = oldValue;
            base.OnValueChanged(oldValue, newValue);
        }
    }
}
