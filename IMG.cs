/*
 * This class is used to load IMG files from Grand Theft Auto San Andreas (gta3.img)
 * We're not saving anything. Just checking what's inside the IMG file and if any textures or models are modified.
 */
using IMGSharp;
using System.Collections.Generic;
using System.Diagnostics;

namespace NostalgiaAnticheat
{
    internal class IMG
    {
        public static Dictionary<string, int> Load(string filePath)
        {
            var files = new Dictionary<string, int>();
            
            IMGArchive img = IMGFile.OpenRead(filePath);

            foreach (IMGArchiveEntry entry in img.Entries)
            {
                if (entry.Name.EndsWith(".txd"))
                {
                    // Send a formatted message to the Debug Console
                    Debug.WriteLine(System.String.Format("{0} ({1} bytes)", entry.Name, entry.Length));

                    // Add the file to the dictionary
                    files.Add(entry.Name, (int)entry.Length);
                }
            }

            return files;
        }
    }
}
