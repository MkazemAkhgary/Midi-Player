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

        private void PlayButton_Open(object sender, RoutedEventArgs e)
        {
            if (!Player.IsLoaded)
                App.OpenFile();
        }
        
        public void Dispose()
        {
            Player.Dispose();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var track = (Track)PlaybackSlider.Template.FindName("PART_Track", PlaybackSlider);
            var thumb = (Thumb)PlaybackSlider.Template.FindName("Thumb", PlaybackSlider);
            var shadow = (ContentControl)PlaybackSlider.Template.FindName("ThumbShadow", PlaybackSlider);
            var selection = (RepeatButton)PlaybackSlider.Template.FindName("PART_SelectionRange", PlaybackSlider);
            var border = (Border)PlaybackSlider.Template.FindName("Border", PlaybackSlider);
            var center = shadow.Width/2;

            DoubleAnimation animation = new DoubleAnimation
            {
                DecelerationRatio = 1,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            Storyboard storyboard = new Storyboard
            {
                Children = new TimelineCollection { animation }
            };
            
            Storyboard.SetTarget(animation, border);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Tag"));

            Action beginAnimation = () =>
            {
                var correction = selection.ActualWidth;
                if (selection.ActualWidth + center > track.ActualWidth) correction -= center;

                var ratio = correction/track.ActualWidth;
                animation.To = ratio;

                storyboard.Begin();
            };
            // todo needs further optimization.
            PlaybackSlider.ValueChanged += (o, args) => beginAnimation();
            thumb.DragStarted += (o, args) => beginAnimation();
            thumb.DragDelta += (o, args) => beginAnimation();
            thumb.DragCompleted += async (o, args) =>
            {
                if(args.Canceled) await Task.Delay(10); // wait for value changes.
                beginAnimation();
            };

            storyboard.Begin();
        }
    }
}
