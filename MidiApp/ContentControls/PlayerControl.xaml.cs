using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace MidiApp.ContentControls
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public sealed partial class PlayerControl : IDisposable
    {
        public PlayerControl()
        {
            InitializeComponent();
        }
        
        public void Dispose()
        {
            Player.Dispose();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AttachAnimation(PlaybackSlider);
        }

        private static void AttachAnimation(RangeBase slider)
        {
            var track = (Track)slider.Template.FindName("PART_Track", slider);
            var thumb = (Thumb)slider.Template.FindName("Thumb", slider);
            var shadow = (Border)slider.Template.FindName("Thumb_Shadow", slider);
            var selection = (RepeatButton)slider.Template.FindName("PART_SelectionRange", slider);
            var border = (Border)slider.Template.FindName("Track_Border", slider);
            var center = shadow.Width / 2;

            var animation = new DoubleAnimation
            {
                To = selection.ActualWidth / track.ActualWidth,
                DecelerationRatio = 1,
                AccelerationRatio = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            var storyboard = new Storyboard
            {
                Children = new TimelineCollection { animation }
            };

            Storyboard.SetTarget(animation, border);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Tag"));

            Action beginAnimation = () =>
            {
                var ratio = selection.ActualWidth / track.ActualWidth;

                if (animation.To == ratio)
                    return; // to prevent animation freeze when there is no need for animation change.

                animation.To = ratio;

                storyboard.Stop(border);
                storyboard.Begin(border, true);
            };
            
            slider.ValueChanged += async (o, args) =>
            {
                if(thumb.IsDragging) return; // drag delta will handle animation.
                await Task.Delay(10); // wait for value changes
                beginAnimation();
            };
            thumb.DragStarted += (o, args) => beginAnimation();
            thumb.DragDelta += (o, args) => beginAnimation();
            thumb.DragCompleted += async (o, args) =>
            {
                if (args.Canceled) await Task.Delay(10); // wait for value changes
                beginAnimation();
            };

            // at startup initialize values.
            border.Tag = selection.ActualWidth / track.ActualWidth;
        }
    }
}
