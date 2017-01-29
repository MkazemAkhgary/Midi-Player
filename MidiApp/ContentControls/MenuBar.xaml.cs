using System.Windows;
using static System.Windows.Application;

namespace MidiApp.ContentControls
{
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class MenuBar
    {
        private MidiPlayer.PlayerComponents.MidiPlayer MidiPlayer => (MidiPlayer.PlayerComponents.MidiPlayer)DataContext;

        public MenuBar()
        {
            InitializeComponent();
        }
        
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void MenuItem_DeviceInfo(object sender, RoutedEventArgs e)
        {
            string info = MidiPlayer.GetMidiOutputDeviceInfo;

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
