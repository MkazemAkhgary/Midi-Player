using System.Windows;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors.Composite
{
    public sealed partial class SliderCompositeBehavior
    {
        private static readonly DependencyPropertyKey ThumbKey =
            DependencyProperty.RegisterAttachedReadOnly(
                nameof(ThumbKey),
                typeof(Thumb),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    default(Thumb),
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        private static readonly DependencyProperty ThumbProperty = ThumbKey.DependencyProperty;

        public static readonly DependencyProperty SourceValueProperty =
            DependencyProperty.Register(
                nameof(SourceValue),
                typeof(double),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSourceValueChanged));

        public static readonly DependencyProperty ValueBindsToSourceProperty =
            DependencyProperty.RegisterAttached(
                "ValueBindsToSource",
                typeof(bool),
                typeof(SliderCompositeBehavior), new PropertyMetadata(true));

        public static readonly DependencyProperty SourceBindsToValueProperty =
            DependencyProperty.RegisterAttached(
                "SourceBindsToValue",
                typeof(bool),
                typeof(SliderCompositeBehavior), new PropertyMetadata(true));

    }
}
