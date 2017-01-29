using System.Windows;
using MidiApp.ViewModel;
using static System.Windows.Application;

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
        
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void MenuItem_DeviceInfo(object sender, RoutedEventArgs e)
        {
            string info = Player.MidiPlayer.GetMidiOutputDeviceInfo;

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
