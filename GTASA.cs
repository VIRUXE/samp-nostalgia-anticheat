using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;

namespace NostalgiaAnticheat
{
    internal class GTASA
    {
        private static ProcessWatchdog processWatchdog;

        public enum GameState
        {
            None,
            GTASA,
            SAMP
        }

        public static int CurrentProcessId = 0;
        public static GameState CurrentState = GameState.None;
        
        // Returns the path to the GTA San Andreas User Files folder
        // If the path doesn't exist, it returns null
        public static string UserFilesPath
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + "\\GTA San Andreas User Files";

                return Directory.Exists(path) ? path : null;
            }
        }

        // Returns the path to the GTA San Andreas installation folder
        // If the path doesn't exist, it returns null
        public static string DefaultInstallPath
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToString() + "\\Rockstar Games\\Grand Theft Auto San Andreas";

                return Directory.Exists(path) ? path : null;
            }
        }

        public static Action<object> OnGameStateChanged { get; internal set; }

        public static void StartWatchDog()
        {
            // Initialize a Watch Dog for the GTA:SA process
            processWatchdog = new ProcessWatchdog("gta_sa");

            processWatchdog.OnProcessOpened += (processId) =>
            {
                CurrentProcessId = processId;
                CurrentState = GameState.GTASA;

                OnGameStateChanged?.Invoke(CurrentState); // 
            };

            processWatchdog.OnProcessClosed += () =>
            {
                CurrentProcessId = 0;
                CurrentState = GameState.None;

                OnGameStateChanged?.Invoke(CurrentState);
            };

            processWatchdog.Start();
        }

        public static string GetExecutablePath()
        {
            Process process = Process.GetProcessById(CurrentProcessId);

            if (process == null) return null;

            return process.MainModule.FileName;
        }

        public static void Close()
        {
            Process process = Process.GetProcessById(CurrentProcessId);

            // Don't do anything if it doesn't exist
            if (process == null) return;

            process.Kill();
        }

        public static Bitmap TakeScreenshot()
        {
            // Grab the GTA SA process
            Process process = Process.GetProcessById(CurrentProcessId);

            // If the process doesn't exist, return
            if (process == null) return null;

            // Grab the window handle
            IntPtr windowHandle = process.MainWindowHandle;

            Bitmap screenshot = Screenshot.CaptureWindow(windowHandle);

            screenshot.Save("game.png", ImageFormat.Png);

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bitmap.Save("fullscreen.png", ImageFormat.Png);
            }

            return screenshot;
        }

        // Get the executable's modules
        public static ProcessModuleCollection GetModules()
        {
            Process process = Process.GetProcessById(CurrentProcessId);

            if (process == null)
                return null;

            return process.Modules;
        }

        public enum GameSettings // byte offsets for each setting
        {
            DrawDistance = 0xB30, // word
            FrameLimiter = 0xB35, // byte (0 = off, 1 = on)
            WideScreen = 0xB34, // byte (0 = off, 1 = on)
            VisualQuality = 0xB2A, // byte (0 = low, 1 = medium, 2 = high, 3 = very high)
            MipMapping = 0xB24, // byte (0 = off, 1 = on)
            Antialiasting = 0xB26, // byte (1 = off, 2 = 1x, 3 = 2x, 4 = 3x)
            Resolution = 0xB36 // byte (3 = 720 x 576, 16 Bit, 4 = 800 x 600, 16 Bit, 7 = 1024 x 768, 16 Bit, 8 = 640 x 480, 32 Bit, 10 = 720 x 576, 32 Bit, 11 = 800 x 600, 32 Bit, 14 = 1024 x 768, 32 Bit
        }
        
        // Get the contents from "gta_sa.set" file, which lives inside the user files directory
        // This file stores the game's configuration settings
        // https://gtamods.com/wiki/Gta_sa.set has the byte offsets for each setting
        public static Dictionary<GameSettings, string> GetGameSettings()
        {
            if (UserFilesPath == null)
                return null;

            string path = UserFilesPath + "\\gta_sa.set";

            if (!File.Exists(path))
                return null;

            byte[] bytes = File.ReadAllBytes(path);

            // We're only getting the Display settings

            // KVP settings Dictionary
            Dictionary<GameSettings, string> settings = new Dictionary<GameSettings, string>();

            foreach (GameSettings setting in Enum.GetValues(typeof(GameSettings)))
            {
                // Get the byte offset for the setting
                int offset = (int)setting;

                // Get the value of the setting
                string value = bytes[offset].ToString();

                // Add the setting to the dictionary
                settings.Add(setting, value);

            }

            return settings;
        }
    }
}
