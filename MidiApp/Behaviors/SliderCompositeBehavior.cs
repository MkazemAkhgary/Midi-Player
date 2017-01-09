using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
// ReSharper disable UnusedMember.Global

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

            var thumb = (Thumb)host.GetValue(ThumbProperty);

            if (thumb == null)
            {
                thumb = ((Track)host.Template?.FindName("PART_Track", host))?.Thumb;
                if (thumb != null) host.SetValue(ThumbKey, thumb);
            }

            return thumb;
        }

        #endregion

        #region SourceValue
        
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

            //soruce.SourceValue = (double)args.NewValue;

            if (ValueBindsWithSource(soruce.Host))
            {
                soruce.Host.Value = (double)args.NewValue;
            }
        }

        #endregion

        #region SourceValue-Value Binding

        public static void BindValueWithSource(Slider host, bool value)
        {
            host.SetValue(BindValueWithSourceProperty, value);
        }

        public static bool ValueBindsWithSource(Slider host)
        {
            return (bool)host.GetValue(BindValueWithSourceProperty);
        }

        #endregion
    }
}
