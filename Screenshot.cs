using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaAnticheat
{
    internal class Screenshot
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetClientRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        private static extern IntPtr ClientToScreen(IntPtr hWnd, ref Point point);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetClientRect(handle, ref rect);

            var point = new Point(0, 0);
            ClientToScreen(handle, ref point);

            var bounds = new Rectangle(point.X, point.Y, rect.Right, rect.Bottom);

            // if the window is not maximized, we need to adjust the bounds
            if (GetWindowLong(handle, -16) != 0x1000000)
            {
                bounds.Width += 15;
                bounds.Height += 50;
            }

            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                IntPtr dc = graphics.GetHdc();
                bool success = PrintWindow(handle, dc, 0);
                graphics.ReleaseHdc(dc);
            }

            return result;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }
}
