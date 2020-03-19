using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace USB_Boot2._0
{
    // used sample from https://stackoverflow.com/questions/7704599/eject-usb-device-via-c-sharp
    class USBEject
    {
        public static void EjectDrive(string driveLetter)
        {
            string path = @"\\.\" + driveLetter + @":";
            IntPtr handle = CreateFile(path, 0x80000000 | 0x40000000,
            0x1 | 0x2, IntPtr.Zero, 0x3, 0, IntPtr.Zero);

            if ((long)handle == -1)
            {
                MessageBox.Show("Unable to open drive " + driveLetter);
                return;
            }

            int dummy = 0;

            DeviceIoControl(handle, 0x2D4808, IntPtr.Zero, 0,
                IntPtr.Zero, 0, ref dummy, IntPtr.Zero);

            CloseHandle(handle);

            MessageBox.Show("OK to remove drive.");
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateFile
            (string filename, uint desiredAccess,
                uint shareMode, IntPtr securityAttributes,
                int creationDisposition, int flagsAndAttributes,
                IntPtr templateFile);
        [DllImport("kernel32")]
        private static extern int DeviceIoControl
            (IntPtr deviceHandle, uint ioControlCode,
                IntPtr inBuffer, int inBufferSize,
                IntPtr outBuffer, int outBufferSize,
                ref int bytesReturned, IntPtr overlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}
