using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors
{
    public sealed class SliderCompositeBehavior : CompositeBehavior<Slider>
    {
        private Slider Host => AssociatedObject;

        #region Boundary Descriptor
        
        public static readonly DependencyPropertyDescriptor MaximumPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(RangeBase.MaximumProperty, typeof(Slider));

        public static readonly DependencyPropertyDescriptor MinimumPropertyDescriptor =
            DependencyPropertyDescriptor.FromProperty(RangeBase.MinimumProperty, typeof(Slider));

        #endregion

        #region Thumb Property

        private static readonly DependencyPropertyKey ThumbProperty =
            DependencyProperty.RegisterAttachedReadOnly(
                $"{nameof(SliderCompositeBehavior)}{nameof(Thumb)}",
                typeof(Thumb),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    default(Thumb),
                    FrameworkPropertyMetadataOptions.NotDataBindable));

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
        
        public static readonly DependencyProperty SourceValueProperty =
            DependencyProperty.Register(
                nameof(SourceValue),
                typeof(double),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSourceValueChanged));

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

        public static readonly DependencyProperty BindValueToSourceValue =
            DependencyProperty.Register(
                nameof(BindValueToSource),
                typeof(bool),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.NotDataBindable));

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
