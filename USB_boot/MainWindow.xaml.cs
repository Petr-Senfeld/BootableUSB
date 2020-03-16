using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;
using Microsoft.Win32;
using DiscUtils;

namespace USB_boot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();


            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {

                if (d.IsReady && (d.DriveType == DriveType.Removable))
                {
                    float usbSize = float.Parse(d.TotalSize.ToString()) / 1024 / 1024 / 1024;
                    var StringDrive = $"{d}{d.VolumeLabel}; {usbSize.ToString().Substring(0, 5)} Gb";
                    USBbox.Items.Add(StringDrive);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (.iso)|*.iso"; // Filter files by extension
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBox.Text = dlg.FileName;
            }
        }
    
        private void Button_Create_USB(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageResult = MessageBox.Show("Právě se chystáte  naformátovat USB, všechna data budou nenavrátně ztracena!", "UPOZORNĚNÍ", MessageBoxButton.YesNo);
            if (messageResult == MessageBoxResult.Yes)
            {
                string USBdriveLetter = USBbox.SelectedItem.ToString().Substring(0, 1); // USB drive letter
                // Format.FormatDrive(USBdriveLetter);
                IsoExtraction.ReadIsoFile(TextBox.Text, USBdriveLetter + ":\\");
            }
            else if (messageResult == MessageBoxResult.No)
            {

            }
        }
    }
}



