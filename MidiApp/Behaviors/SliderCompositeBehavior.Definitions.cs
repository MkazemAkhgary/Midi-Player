using System.Windows;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors
{
    public sealed partial class SliderCompositeBehavior
    {
        private static readonly DependencyPropertyKey ThumbKey =
            DependencyProperty.RegisterAttachedReadOnly(
                $"{nameof(SliderCompositeBehavior)}{nameof(Thumb)}",
                typeof(Thumb),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    default(Thumb),
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        public static readonly DependencyProperty ThumbProperty = ThumbKey.DependencyProperty;

        public static readonly DependencyProperty BindValueWithSourceProperty =
            DependencyProperty.RegisterAttached(
                nameof(ValueBindsWithSource),
                typeof(bool),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        public static readonly DependencyProperty SourceValueProperty =
            DependencyProperty.Register(
                nameof(SourceValue),
                typeof(double),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSourceValueChanged));
    }
}
