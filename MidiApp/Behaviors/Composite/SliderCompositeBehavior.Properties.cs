using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

// ReSharper disable MemberCanBePrivate.Global

namespace MidiApp.Behaviors.Composite
{
    public sealed partial class SliderCompositeBehavior
    {
        public double SourceValue
        {
            get { return (double)GetValue(SourceValueProperty); }
            set { SetValue(SourceValueProperty, value); }
        }

        public static void SetSourceValue(Slider host, double value)
        {
            GetReference(host).SetValue(SourceValueProperty, value);
        }

        public static double GetSourceValue(Slider host)
        {
            return (double) GetReference(host).GetValue(SourceValueProperty);
        }

        public static Thumb GetThumb(Slider host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host), $@"{nameof(host)} cant be null.");

            var thumb = (Thumb) GetReference(host).GetValue(ThumbProperty);
            if (thumb == null)
            {
                thumb = ((Track) host.Template?.FindName("PART_Track", host))?.Thumb;
                if (thumb != null) GetReference(host).SetValue(ThumbKey, thumb);
            }
            return thumb;
        }

        public static bool GetValueBindsToSource(Slider host)
        {
            return GetValueByRef<bool>(host, ValueBindsToSourceProperty);
        }

        public static bool GetSourceBindsToValue(Slider host)
        {
            return GetValueByRef<bool>(host, SourceBindsToValueProperty);
        }

        public static void SetValueBindsToSource(Slider host, bool value)
        {
            SetValueByRef(host, ValueBindsToSourceProperty, value);
        }

        public static void SetSourceBindsToValue(Slider host, bool value)
        {
            SetValueByRef(host, SourceBindsToValueProperty, value);
        }
    }
}
