using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace USB_boot
{
    class Format
    {
        public static async Task FormatDriveAsync(string drive) 
        {
            await Task.Run(() =>
            {
                FormatDrive(drive);
            });
        }

        private static void FormatDrive(string drive)
        {
            int indexOfDrive = GetIndexOfDriveAsync(drive);
            Process process = new Process();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = Environment.SystemDirectory;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.StandardInput.WriteLine("select disk " + indexOfDrive);
            process.StandardInput.WriteLine("clean");
            process.StandardInput.WriteLine("convert mbr");
            process.StandardInput.WriteLine("create partition primary");
            process.StandardInput.WriteLine("select partition 1");
            process.StandardInput.WriteLine("format FS=FAT32 label=Windows10 quick");
            process.StandardInput.WriteLine("assign letter=" + drive);
            process.StandardInput.WriteLine("exit");
            // string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        public static int GetIndexOfDriveAsync(string drive)
        {
            // execute DiskPart to get list of volumes
            Process process = new Process();
            process.StartInfo.FileName = "diskpart.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.StandardInput.WriteLine("list volume");
            process.StandardInput.WriteLine("exit");
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // extract information from output
            string table = output.Split(new string[] { "DISKPART>" }, StringSplitOptions.None)[1];
            var rows = table.Split(new string[] { "\n" }, StringSplitOptions.None);
            List<string> letters = new List<string>();
            for (int i = 3; i < rows.Length; i++)
            {
                if (rows[i].Contains("Volume"))
                {
                    letters.Add(rows[i].Split(new string[] { " " }, StringSplitOptions.None)[8]);
                }
            }
            letters.RemoveAll(x => x == "");
            int result = letters.IndexOf(drive);
            return result;
        }
    }
}
