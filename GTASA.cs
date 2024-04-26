using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using System.Threading;

namespace NostalgiaAnticheat {
    internal class GTASA {
        private static ProcessWatchdog processWatchdog;

        public enum GameState { None, GTASA, SAMP }

        public static int CurrentProcessId = 0;
        public static GameState CurrentState = GameState.None;

        // Returns the path to the GTA San Andreas User Files folder
        // If the path doesn't exist, it returns null
        public static string UserFilesPath {
            get {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + "\\GTA San Andreas User Files";

                return Directory.Exists(path) ? path : null;
            }
        }

        // Returns the path to the GTA San Andreas installation folder
        // If the path doesn't exist, it returns null
        public static string DefaultInstallPath {
            get {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToString() + "\\Rockstar Games\\Grand Theft Auto San Andreas";

                return Directory.Exists(path) ? path : null;
            }
        }
        public static Action<object> OnGameStateChanged { get; internal set; }

        public static void StartWatchDog() {
            // Initialize a Watch Dog for the GTA:SA process
            processWatchdog = new ProcessWatchdog("gta_sa");

            processWatchdog.OnProcessOpened += (processId) => {
                if (CurrentProcessId != 0) Process.GetProcessById(processId).Kill();
                if (CurrentState != GameState.None) return;

                CurrentProcessId = processId;
                CurrentState = GameState.GTASA;

                OnGameStateChanged?.Invoke(CurrentState);

                // Create a Thread to loop check the game modules to see when SA-MP get's injected or not
                new Thread(() => {
                    while (CurrentState != GameState.None) {
                        ProcessModuleCollection modules = GetModules();

                        if (modules == null) return;

                        foreach (ProcessModule module in modules) {
                            if (module.ModuleName == "samp.dll") {
                                if (CurrentState == GameState.SAMP) break; // Don't change the state if it's already SAMP

                                CurrentState = GameState.SAMP;

                                OnGameStateChanged?.Invoke(CurrentState);

                                break;
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }).Start();
            };

            processWatchdog.OnProcessClosed += () => {
                if (Process.GetProcessesByName("gta_sa").Length > 1) return; // Don't change the state if there's another GTA:SA process running
                if (CurrentState == GameState.None) return;

                CurrentProcessId = 0;
                CurrentState = GameState.None;

                OnGameStateChanged?.Invoke(CurrentState);
            };

            processWatchdog.Start();
        }

        public static Process GetProcess() => Process.GetProcessById(CurrentProcessId);

        public static string GetExecutablePath() => Process.GetProcessById(CurrentProcessId)?.MainModule.FileName;

        public static void Close() => Process.GetProcessById(CurrentProcessId)?.Kill();

        // Get the executable's modules
        public static ProcessModuleCollection GetModules() => Process.GetProcessById(CurrentProcessId)?.Modules;

        public static Bitmap TakeScreenshot() {
            // Grab the GTA SA process
            Process process = Process.GetProcessById(CurrentProcessId);

            // If the process doesn't exist, return
            if (process == null) return null;

            Bitmap screenshot = Screenshot.CaptureWindow(process.MainWindowHandle);

            screenshot.Save("game.png", ImageFormat.Png);

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height)) {
                using (Graphics g = Graphics.FromImage(bitmap)) g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                bitmap.Save("fullscreen.png", ImageFormat.Png);
            }

            return screenshot;
        }

        public enum GameSettings { // byte offsets for each setting
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
        public static Dictionary<GameSettings, string> GetGameSettings() {
            if (UserFilesPath == null) return null;

            string path = UserFilesPath + "\\gta_sa.set";

            if (!File.Exists(path)) return null;

            byte[] bytes = File.ReadAllBytes(path);

            // We're only getting the Display settings

            // KVP settings Dictionary
            Dictionary<GameSettings, string> settings = new Dictionary<GameSettings, string>();

            foreach (GameSettings setting in Enum.GetValues(typeof(GameSettings))) settings.Add(setting, bytes[(int)setting].ToString());

            return settings;
        }
    }
}
