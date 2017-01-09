using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors
{
    public sealed partial class SliderCompositeBehavior : CompositeBehavior<Slider>
    {
        private Slider Host => AssociatedObject;

        #region Boundary Descriptor
        
        public static readonly DependencyPropertyDescriptor MaximumPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(RangeBase.MaximumProperty, typeof(Slider));

        public static readonly DependencyPropertyDescriptor MinimumPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(RangeBase.MinimumProperty, typeof(Slider));

        #endregion

        #region Thumb Property

        public static Thumb GetThumb(Slider host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host), $@"{nameof(host)} cant be null.");

            var thumb = (Thumb)host.GetValue(ThumbProperty.DependencyProperty);

            if (thumb == null)
            {
                thumb = ((Track)host.Template?.FindName("PART_Track", host))?.Thumb;
                if (thumb != null) host.SetValue(ThumbProperty, thumb);
            }

            return thumb;
        }

        #endregion

        #region Source Value
        
        public double SourceValue
        {
            get { return (double)Host.GetValue(SourceValueProperty); }
            set { Host.SetValue(SourceValueProperty, value); }
        }

        public static void SetSourceValue(Slider host, double value)
        {
            host.SetValue(SourceValueProperty, value);
        }

        public static double GetSourceValue(Slider host)
        {
            return (double)host.GetValue(SourceValueProperty);
        }

        private static void OnSourceValueChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var soruce = (SliderCompositeBehavior)dpo;

            if (soruce.BindValueToSource)
            {
                soruce.Host.Value = (double)args.NewValue;
            }
        }

        #endregion

        #region Bind Value to Source

        public bool BindValueToSource
        {
            get { return (bool)Host.GetValue(BindValueToSourceValue); }
            set { Host.SetValue(BindValueToSourceValue, value); }
        }

        public static void SetBindValueToSource(Slider host, bool value)
        {
            host.SetValue(BindValueToSourceValue, value);
        }

        public static bool GetBindValueToSource(Slider host)
        {
            return (bool)host.GetValue(BindValueToSourceValue);
        }

        #endregion
    }
}
