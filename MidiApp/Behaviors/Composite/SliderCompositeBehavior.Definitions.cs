using System.Windows;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors.Composite
{
    public sealed partial class SliderCompositeBehavior
    {
        private static readonly DependencyPropertyKey TrackKey =
            DependencyProperty.RegisterAttachedReadOnly(
                nameof(TrackKey),
                typeof(Track),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    default(Track),
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        private static readonly DependencyProperty TrackProperty = TrackKey.DependencyProperty;

        public static readonly DependencyProperty SourceValueProperty =
            DependencyProperty.Register(
                nameof(SourceValue),
                typeof(double),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSourceValueChanged));

        private static readonly DependencyProperty ValueBindsToSourceProperty =
            DependencyProperty.RegisterAttached(
                "ValueBindsToSource",
                typeof(bool),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        private static readonly DependencyProperty SourceBindsToValueProperty =
            DependencyProperty.RegisterAttached(
                "SourceBindsToValue",
                typeof(bool),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.NotDataBindable));
    }
}
