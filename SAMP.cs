using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace NostalgiaAnticheat {
    internal class SAMP {
        // HKEY_CURRENT_USER\Software\SAMP
        public static string GamePath {
            get {
                return GetRegistryValue("gta_sa_exe").Replace("gta_sa.exe", "");
            }
        }

        public static string PlayerName {
            get {
                return GetRegistryValue("PlayerName");
            }
        }

        public static string Version {
            get {
                string path = GamePath + "samp.dll";

                if (File.Exists(path)) {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);
                    return fvi.FileVersion;
                } else {
                    return null;
                }
            }
        }

        public static bool IsInstalled {
            get {
                return GetRegistryValueNames() != null;
            }
        }

        public static bool IsRunning {
            get {
                // GTA:SA's Modules and check if "samp.dll" is loaded
                ProcessModuleCollection modules = GTASA.GetModules();

                if (modules == null) return false;

                foreach (ProcessModule module in modules) if (module.ModuleName == "samp.dll") return true;

                return false;
            }
        }

        public static string[] GetRegistryValueNames() => Registry.CurrentUser.OpenSubKey("Software\\SAMP")?.GetValueNames();

        private static string GetRegistryValue(string value) => Registry.CurrentUser.OpenSubKey("Software\\SAMP")?.GetValue(value)?.ToString();

        // "gpci" function as seen in SA-MP, but in C#
        // "gpci" stands for Get Player Computer Id
        // This is linked to their SAMP/GTA on their computer
        // It is a non-reversible(lossy) hash derived from information about your San Andreas installation path.
        // Returns a string with a maximum of 40 characters
		public static string GetPlayerComputerId() {
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GTA San Andreas User Files", "SAMP", "gta_sa.set");

			if (!File.Exists(path)) return string.Empty;

			foreach (string line in File.ReadLines(path)) {
				if (line.Contains("gta_sa.exe")) {
					string[] split = line.Split('=');
					return split.Length > 1 ? split[1].Trim() : string.Empty;
				}
			}

			return string.Empty;
		}
    }
}
