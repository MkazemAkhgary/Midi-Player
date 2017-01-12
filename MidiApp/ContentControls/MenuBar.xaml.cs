using System.Windows;
using MidiPlayer.PlayerComponents;
using static System.Windows.Application;
using static MidiApp.App;

namespace MidiApp.ContentControls
{
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class MenuBar
    {
        private Player Player => (Player)DataContext;

        public MenuBar()
        {
            InitializeComponent();
        }

        private void MenuItem_Open(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenFile();
            Player.Start();
        }

        private void MenuItem_Stop(object sender, RoutedEventArgs e)
        {
            Player.Stop();
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void MenuItem_DeviceInfo(object sender, RoutedEventArgs e)
        {
            string info = Player.GetMidiOutputDeviceInfo;

            MessageBox.Show(
                info,
                "Midi Output Device Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information,
                MessageBoxResult.OK);
        }

        private void MenuItem_About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Programmer : " +
                "L.van.Beethoven9@gmail.com",
                "About",
                MessageBoxButton.OK,
                MessageBoxImage.Information,
                MessageBoxResult.OK);
        }
    }
}
