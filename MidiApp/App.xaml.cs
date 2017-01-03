using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;

namespace MidiApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly Lazy<PlayerControl> Ctrl;
        private static PlayerControl Control => Ctrl.Value;

        static App()
        {
            Ctrl = new Lazy<PlayerControl>(() => (Current.MainWindow as MainWindow)?.PlayerControl);
        }

        internal static void OpenFile()
        {
            if ((Current.FindResource("FileDialog") as OpenFileDialog)?.ShowDialog(Current.MainWindow) != true)
            {
                Control.Player.Close();
            }
        }

        internal static void LoadStream(string filename)
        {
            Control.Player.Close();

            try
            {
                Control.Player.Open(filename);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Control.Player.Close();
            }
        }

        private void FileDialog_OnFileOk(object sender, CancelEventArgs e)
        {
            var opd = sender as OpenFileDialog;
            if(opd == null) return;
            LoadStream(opd.FileName);
        }
    }
}
