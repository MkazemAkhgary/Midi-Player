using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using MidiPlayer.PlayerComponents;

namespace MidiApp
{
    using ContentControls;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly Lazy<PlayerControl> Ctrl;
        private static Player Player => Ctrl.Value.Player;
        private new static MainWindow MainWindow => Current.MainWindow as MainWindow;
        private static OpenFileDialog OpenFileDialog => Current.FindResource("FileDialog") as OpenFileDialog;

        static App()
        {
            Ctrl = new Lazy<PlayerControl>(() => MainWindow?.PlayerControl);
        }

        internal static void OpenFile()
        {
            if (OpenFileDialog?.ShowDialog(Current.MainWindow) != true)
            {
                Ctrl.Value.ToggleButton.IsChecked = false;
            }
        }

        internal static async Task LoadStream(string filename)
        {
            try
            {
                await Player.Open(filename);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
$@"{exception.Message}
{exception.StackTrace}", "Error!");

                Player.Close();
            }
        }

        private async void FileDialog_OnFileOk(object sender, CancelEventArgs e)
        {
            var opd = sender as OpenFileDialog;
            if(opd == null) return;
            await LoadStream(opd.FileName);
        }
    }
}
