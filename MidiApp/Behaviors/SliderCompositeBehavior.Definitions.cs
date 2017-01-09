using System.Windows;
using System.Windows.Controls.Primitives;

namespace MidiApp.Behaviors
{
    public sealed partial class SliderCompositeBehavior
    {
        private static readonly DependencyPropertyKey ThumbProperty =
            DependencyProperty.RegisterAttachedReadOnly(
                $"{nameof(SliderCompositeBehavior)}{nameof(Thumb)}",
                typeof(Thumb),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    default(Thumb),
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
        
        public static readonly DependencyProperty BindValueToSourceValue =
            DependencyProperty.Register(
                nameof(BindValueToSource),
                typeof(bool),
                typeof(SliderCompositeBehavior),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.NotDataBindable));
    }
}
