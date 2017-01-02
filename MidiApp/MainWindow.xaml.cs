using System.ComponentModel;
using System.Windows;

namespace MidiUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var data = e.Data.GetData(DataFormats.FileDrop) as string[];

                App.LoadStream(data?[0]);
            }
        }

        private void MenuItem_Open(object sender, RoutedEventArgs routedEventArgs)
        {
            App.OpenFile();
            PlayerControl.Player.Start();
        }

        private void MenuItem_Stop(object sender, RoutedEventArgs e)
        {
            PlayerControl.Player.Stop();
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            PlayerControl.Dispose();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string info = PlayerControl.Player.GetMidiOutputDeviceInfo;

            MessageBox.Show(
                this, info, "Midi Output Device Info",
                MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }
    }
}