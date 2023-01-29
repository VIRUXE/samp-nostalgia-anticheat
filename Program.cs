using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace NostalgiaAnticheat
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(false, "Global\\" + Assembly.GetExecutingAssembly().GetType().GUID.ToString()))
            {
                if (!mutex.WaitOne(0, false)) return;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());

                mutex.ReleaseMutex();
            }
        }
    }
}
