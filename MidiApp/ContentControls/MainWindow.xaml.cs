using System.ComponentModel;
using System.Windows;

namespace MidiApp.ContentControls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = PlayerControl.Player;
        }

        private void Window_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var data = e.Data.GetData(DataFormats.FileDrop) as string[];
                PlayerControl.Player.Open.RaiseCommand(data);
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            PlayerControl.Dispose();
        }
    }
}