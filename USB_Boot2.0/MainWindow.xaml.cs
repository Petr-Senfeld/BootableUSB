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

        private async void CreateUsb()
        {
            // getting warning message before start of Formatting
            MessageBoxResult messageResult = MessageBox.Show("Právě se chystáte  naformátovat USB, všechna data budou nenavrátně ztracena!", "UPOZORNĚNÍ", MessageBoxButton.YesNo);
            if (messageResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Work with XML within the app directory
            string XMLFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"data\config.xml");
            XmlDocument doc = new XmlDocument();
            string isoPath;
            try
            {
                doc.Load(XMLFilePath);
                isoPath = Path.Combine(Directory.GetCurrentDirectory(), doc.DocumentElement.SelectSingleNode("//zobecesitousbtool/esiimagefile").Attributes["path"].Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nepodařilo se načíst konfiguraci v config.xml\n{ex}");
                return;
            }

            // Check if the isoPath is correct
            if (!File.Exists(isoPath))
            {
                MessageBox.Show("Cesta" + isoPath + " je neplatná, upravte prosím 'config.xlm'. Program bude ukončen.");
                return;
            }

            // New window as "loading window"
            Waiting_window window = new Waiting_window();
            window.Show();

            // USB drive letter
            string USBdriveLetter = USBbox.SelectedItem.ToString().Substring(0, 1);

            // Fortmating drive
            try
            {
                await Format.FormatDriveAsync(USBdriveLetter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při formátování jednotky\n{ex}");
                return;
            }

            // Method to extract ISO to selected USB
            try
            {
                await IsoExtraction.ReadIsoFileAsync(isoPath, USBdriveLetter + ":\\");
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při zápisu iso\n{ex}");
                return;
            }
            window.Close();
            MessageBox.Show("Hotovo");

            // Method to Eject drive after work is done
            //USBEject.EjectDrive(USBdriveLetter);
        }

        private void Button_Create_USB(object sender, RoutedEventArgs e)
        {
            CreateUsb();
        }
    }
}