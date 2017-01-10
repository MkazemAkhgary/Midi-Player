using System.Windows;
using System.Windows.Controls;
// ReSharper disable UnusedMember.Global

namespace MidiApp.Behaviors.Composite
{
    public sealed partial class SliderCompositeBehavior : CompositeBehavior<Slider>
    {
        protected override void OnSelfAttached()
        {
            Host.ValueChanged += OnValueChanged;
        }

        protected override void OnSelfDetaching()
        {
            Host.ValueChanged -= OnValueChanged;
        }

        private static void OnSourceValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var soruce = (SliderCompositeBehavior)d;
            var newval = (double) e.NewValue;
            var oldval = (double) e.OldValue;
            var args = new RoutedPropertyChangedEventArgs<double>(oldval, newval);
            SetSourceValue(soruce.Host, newval);
            soruce.OnSourceValueChanged(soruce, args);
        }

        private void OnSourceValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GetValueBindsToSource(Host))
            {
                Host.Value = e.NewValue;
            }
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GetSourceBindsToValue(Host))
            {
                SourceValue = e.NewValue;
            }
        }
    }
}
