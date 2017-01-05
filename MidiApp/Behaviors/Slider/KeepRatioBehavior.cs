using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors.Slider
{
    using System.Windows.Controls;

    public sealed class KeepRatioBehavior : SliderBehavior
    {
        private static readonly DependencyPropertyDescriptor MaximumPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(RangeBase.MaximumProperty, typeof(Slider));

        private static readonly DependencyPropertyDescriptor MinimumPropertyDescriptor =
             DependencyPropertyDescriptor.FromProperty(RangeBase.MinimumProperty, typeof(Slider));

        private double _oldMax;
        private double _oldMin;
        private double _oldVal;

        protected override void OnBehaviorAttached()
        {
            MaximumPropertyDescriptor?.AddValueChanged(AssociatedObject, OnMaximumChanged);
            MinimumPropertyDescriptor?.AddValueChanged(AssociatedObject, OnMinimumChanged);

            AssociatedObject.ValueChanged += OnValueChanged;

            _oldMax = (double) AssociatedObject.GetValue(RangeBase.MaximumProperty);
            _oldMin = (double) AssociatedObject.GetValue(RangeBase.MinimumProperty);
            _oldVal = (double) AssociatedObject.GetValue(RangeBase.ValueProperty);
        }

        protected override void OnBehaviorDetaching()
        {
            MaximumPropertyDescriptor?.RemoveValueChanged(AssociatedObject, OnMaximumChanged);
            MinimumPropertyDescriptor?.RemoveValueChanged(AssociatedObject, OnMinimumChanged);

            AssociatedObject.ValueChanged -= OnValueChanged;
        }

        private void OnMaximumChanged(object sender, EventArgs eventArgs)
        {
            if (Maximum > Value) // use original value if slider did not touch Value, (i.e when Value = newMax)
                _oldVal = Value;

            Value = _oldVal * (Maximum - Minimum) / (_oldMax - Minimum);

            _oldMax = Maximum;
        }

        private void OnMinimumChanged(object sender, EventArgs eventArgs)
        {
            if (Minimum < Value)
                _oldVal = Value;

            Value = _oldVal * (Maximum - Minimum) / (Maximum - _oldMin);

            _oldMin = Minimum;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _oldVal = e.OldValue;
        }
    }
}
