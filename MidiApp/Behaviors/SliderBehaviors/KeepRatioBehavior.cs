using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using MidiApp.Behaviors.Composite;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// Keeps the ratio of slider Value when ever boundary changes.
    /// </summary>
    public sealed class KeepRatioBehavior : Behavior<Slider>
    {
        private static readonly DependencyPropertyDescriptor MaximumPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(
                RangeBase.MaximumProperty,
                typeof(Slider));

        private static readonly DependencyPropertyDescriptor MinimumPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(
                RangeBase.MinimumProperty,
                typeof(Slider));

        private Slider Host => AssociatedObject;

        private double Maximum => Host.Maximum;
        private double Minimum => Host.Minimum;
        private double Value
        {
            get { return Host.Value; }
            set { Host.Value = value; }
        }

        private double _oldMax;
        private double _oldMin;
        private double _oldVal;

        protected override void OnAttached()
        {
            MaximumPropertyDescriptor.AddValueChanged(Host, OnMaximumChanged);
            MinimumPropertyDescriptor.AddValueChanged(Host, OnMinimumChanged);

            _oldMax = Host.Maximum;
            _oldMin = Host.Minimum;
            _oldVal = Host.Value;

            Host.ValueChanged += OnValueChanged;
        }

        protected override void OnDetaching()
        {
            MaximumPropertyDescriptor.RemoveValueChanged(Host, OnMaximumChanged);
            MinimumPropertyDescriptor.RemoveValueChanged(Host, OnMinimumChanged);

            _oldMax = 1d;
            _oldMin = 0d;
            _oldVal = 0d;

            Host.ValueChanged -= OnValueChanged;
        }

        private void OnMaximumChanged(object sender, EventArgs args)
        {
            if (Maximum > Value) _oldVal = Value; // use new value if Value is not limited to maximum.
            Value = _oldVal * (Maximum - Minimum) / (_oldMax - Minimum);
            SliderCompositeBehavior.SetSourceValue(Host, Value);
            _oldMax = Maximum;
        }

        private void OnMinimumChanged(object sender, EventArgs args)
        {
            if (Minimum < Value) _oldVal = Value;
            Value = _oldVal * (Maximum - Minimum) / (Maximum - _oldMin);
            SliderCompositeBehavior.SetSourceValue(Host, Value);
            _oldMin = Minimum;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            _oldVal = args.OldValue;
        }
    }
}
