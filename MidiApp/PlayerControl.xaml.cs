using System;
using System.Windows;

namespace MidiUI
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
            if (!Player.Context.IsPlaybackLoaded)
                App.OpenFile();
        }
        
        public void Dispose()
        {
            Player.Dispose();
        }
    }
}
