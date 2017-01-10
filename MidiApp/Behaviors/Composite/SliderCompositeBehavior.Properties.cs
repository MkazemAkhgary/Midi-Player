using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

// ReSharper disable UnusedMember.Global

namespace MidiApp.Behaviors.Composite
{
    public sealed partial class SliderCompositeBehavior
    {
        private Slider Host => AssociatedObject;
        private Thumb Thumb => GetThumb(Host);

        #region Reference Properties
        
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

        #endregion

        #region Static Properties

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

        public static void SetValueBindsToSource(Slider host, bool value)
        {
            host.SetValue(ValueBindsToSourceProperty, value);
        }

        public static bool GetValueBindsToSource(Slider host)
        {
            return (bool) host.GetValue(ValueBindsToSourceProperty);
        }

        public static void SetSourceBindsToValue(Slider host, bool value)
        {
            host.SetValue(SourceBindsToValueProperty, value);
        }

        public static bool GetSourceBindsToValue(Slider host)
        {
            return (bool)host.GetValue(SourceBindsToValueProperty);
        }

        #endregion
    }
}
