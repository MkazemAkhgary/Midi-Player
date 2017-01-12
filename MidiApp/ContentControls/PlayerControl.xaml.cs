using System;
using System.Windows;

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
    }
}
