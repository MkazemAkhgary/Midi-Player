using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MidiApp.Behaviors.SliderBehaviors
{
    /// <summary>
    /// Keeps the ratio of slider Value when ever boundary changes.
    /// </summary>
    public sealed class KeepRatioBehavior : Behavior<Slider>
    {
        private Slider Host => AssociatedObject;

        private double Maximum => Host.Maximum;
        private double Minimum => Host.Minimum;
        private double Value
        {
            get { return Host.Value; }
            set { Host.Value = value; }
        }

        private double _oldMax = 10;
        private double _oldMin = 0;
        private double _oldVal = 0;

        protected override void OnAttached()
        {
            SliderCompositeBehavior.MaximumPropertyDescriptor.AddValueChanged(Host, OnMaximumChanged);
            SliderCompositeBehavior.MinimumPropertyDescriptor.AddValueChanged(Host, OnMinimumChanged);

            Host.ValueChanged += OnValueChanged;
        }

        protected override void OnDetaching()
        {
            SliderCompositeBehavior.MaximumPropertyDescriptor.RemoveValueChanged(Host, OnMaximumChanged);
            SliderCompositeBehavior.MinimumPropertyDescriptor.RemoveValueChanged(Host, OnMinimumChanged);

            Host.ValueChanged -= OnValueChanged;
        }

        private void OnMaximumChanged(object sender, EventArgs args)
        {
            if (Maximum > Value) _oldVal = Value; // use new value if Value is not limited to maximum.
            Value = _oldVal * (Maximum - Minimum) / (_oldMax - Minimum);
            _oldMax = Maximum;
        }

        private void OnMinimumChanged(object sender, EventArgs args)
        {
            if (Minimum < Value) _oldVal = Value;
            Value = _oldVal * (Maximum - Minimum) / (Maximum - _oldMin);
            _oldMin = Minimum;
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            _oldVal = args.OldValue;
        }
    }
}
