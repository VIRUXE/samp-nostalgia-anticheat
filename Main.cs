using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NostalgiaAnticheat {
    public partial class Main : Form {
        public Main() {
            InitializeComponent();

            GTASA.StartWatchDog();

            GTASA.OnGameStateChanged += (state) => Invoke(new Action(() => DisplayGameState()));

            lblGTAState.Text = "Estado do GTA:SA: Nenhum";

            new Thread(() => {
                while (true) {
                    Thread.Sleep(1000);
                    foreach (Process process in Process.GetProcesses()) {
                        long memory = process.WorkingSet64 / 1024 / 1024; // Convert to MB

                        // GTA:SA process uses between 310 and 320 MB of memory
                        if (memory > 310 && memory < 320) Debug.WriteLine("{0} - {1} MB", process.ProcessName, memory);
                    }
                }
            }).Start();
        }

        private void Main_Load(object sender, EventArgs e) {
        }

        // Event for when the application closes
        private void Main_FormClosing(object sender, FormClosingEventArgs e) {
            // Close the GTA:SA process if it's running
            if (GTASA.CurrentState != GTASA.GameState.None) GTASA.Close();
        }

        private void DisplayGameState() {
            string prefix = "Estado do GTA:SA: ";

            switch (GTASA.CurrentState) {
                case GTASA.GameState.None:
                    lblGTAState.Text = prefix + "Nenhum";
                    break;
                case GTASA.GameState.GTASA:
                    lblGTAState.Text = prefix + "Rodando";
                    break;
                case GTASA.GameState.SAMP:
                    lblGTAState.Text = prefix + "Rodando (SAMP)";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            //listBox1.Items.Add(SAMP.IsRunning ? "SAMP is running" : "SAMP is not running");
            string settingsStr = String.Empty;
            Dictionary<GTASA.GameSettings, string> settings = GTASA.GetGameSettings();

            foreach (KeyValuePair<GTASA.GameSettings, string> setting in settings) settingsStr += setting.Key.ToString() + ": " + setting.Value.ToString() + Environment.NewLine;

            MessageBox.Show(settingsStr, "GTA:SA Settings");
        }

        private void button2_Click(object sender, EventArgs e) {
            ProcessModuleCollection modules = GTASA.GetModules();
            List<string> moduleNames = new List<string>();

            /*listBox2.Items.Clear();
            foreach (ProcessModule module in modules)
            {
                listBox2.Items.Add(module.ModuleName);
                moduleNames.Add(module.ModuleName);
            }*/

            // Send an HTTP UPLOAD to http://localhost/nostalgia/ac.php with the module names
            // Setup the POST data
            Dictionary<string, string> postData = new Dictionary<string, string> {
                { "modules", String.Join(",", moduleNames.ToArray()) },
                { "player", SAMP.PlayerName },
                { "version", SAMP.Version.ToString() }
            };

            // Send the POST web request
            var request = WebRequest.Create("https://webhook.site/8bca59e4-5496-465c-b532-cb3875766c3f");

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // Encode the POST data
            string encodedData = String.Empty;
            foreach (KeyValuePair<string, string> pair in postData) encodedData += WebUtility.UrlEncode(pair.Key) + "=" + WebUtility.UrlEncode(pair.Value) + "&";

            // Remove the last ampersand
            encodedData = encodedData.Substring(0, encodedData.Length - 1);

            // Write the POST data to the request stream
            byte[] data = Encoding.ASCII.GetBytes(encodedData);
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);

            // Get the response
            var response = (HttpWebResponse)request.GetResponse();

            // Read the response
            string responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();

            // Show the response
            MessageBox.Show(responseString, "Response");

            // Close the response
            response.Close();
        }

        private void button4_Click(object sender, EventArgs e) {
            // Load the GTA:SA IMG file

            string path = SAMP.GamePath + "models\\gta3.img";

            var files = IMG.Load(path);

            /*listBox3.Items.Clear();

            foreach (var file in files)
            {
                listBox3.Items.Add(String.Format("{0} - {1}", file.Key, file.Value));
            }*/
        }
    }
}
