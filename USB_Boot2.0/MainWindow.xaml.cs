using System;
using System.IO;
using System.Windows;
using Path = System.IO.Path;
using System.Xml;
using USB_Boot2._0;

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
            
            // Getting all the removable drives
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

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.Filter = "Image files (.iso)|*.iso"; // Filter files by extension
        //    Nullable<bool> result = dlg.ShowDialog();
        //    if (result == true)
        //    {
        //        TextBox.Text = dlg.FileName;
        //    }
        //}

        private void Button_Create_USB(object sender, RoutedEventArgs e)
        {
            // getting warning message before start of Formatting
            MessageBoxResult messageResult = MessageBox.Show("Právě se chystáte  naformátovat USB, všechna data budou nenavrátně ztracena!", "UPOZORNĚNÍ", MessageBoxButton.YesNo);
            if (messageResult == MessageBoxResult.Yes)
            {
                string USBdriveLetter = USBbox.SelectedItem.ToString().Substring(0, 1); // USB drive letter
                // Fortmating drive
                Format.FormatDrive(USBdriveLetter);
                // Work with XML within the app directory
                string XMLFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"data\config.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(XMLFilePath);
                string isoPath = Path.Combine(Directory.GetCurrentDirectory(), doc.DocumentElement.SelectSingleNode("//zobecesitousbtool/esiimagefile").Attributes["path"].Value);
                // MessageBox.Show(isoPath);
                // New window as "loading window"
                Waiting_window window = new Waiting_window();
                window.Show();
                // Method to extract ISO to selected USB
                IsoExtraction.ReadIsoFile(isoPath, USBdriveLetter + ":\\");
                window.Close();
                // Method to Eject drive after work is done
                USBEject.EjectDrive(USBdriveLetter);
                Application.Current.Shutdown();
            }
            else if (messageResult == MessageBoxResult.No)
            {
                Application.Current.Shutdown();
            }
        }
    }
}



