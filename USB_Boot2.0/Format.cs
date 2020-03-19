using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace USB_boot
{
    class Format
    {
        public static bool FormatDrive(string driveLetter, string label = "Windows10", string fileSystem = "FAT32", bool quickFormat = true, bool enableCompression = false, int? clusterSize = null)
        {
            bool success = false;
            string drive = driveLetter + ":";
            try
            {
                var di = new DriveInfo(drive);
                var psi = new ProcessStartInfo();
                psi.FileName = "format.com";
                psi.CreateNoWindow = true; //if you want to hide the window
                psi.WorkingDirectory = Environment.SystemDirectory;
                // following commands can be adjusted to expected need
                // **************************************************
                psi.Arguments = "/FS:" + fileSystem +
                                             " /Y" +
                                             " /V:" + label +
                                             (quickFormat ? " /Q" : "") +
                                             ((fileSystem == "FAT32" && enableCompression) ? " /C" : "") +
                                             (clusterSize.HasValue ? " /A:" + clusterSize.Value : "") +
                                             " " + drive;
                // **************************************************
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                var formatProcess = Process.Start(psi);
                var swStandardInput = formatProcess.StandardInput;
                swStandardInput.WriteLine();
                formatProcess.WaitForExit();
                success = true;
            }
            catch (Exception) { }
            return success;
        }
    }
}
