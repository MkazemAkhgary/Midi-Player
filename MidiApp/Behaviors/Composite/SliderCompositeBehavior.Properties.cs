﻿using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        public static Track GetTrack(Slider host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host), $@"{nameof(host)} cant be null.");

            var track = (Track)GetReference(host).GetValue(TrackProperty);
            if (track == null)
            {
                track = (Track)host.Template?.FindName("PART_Track", host);
                if (track != null) GetReference(host).SetValue(TrackKey, track);
            }
            return track;
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
