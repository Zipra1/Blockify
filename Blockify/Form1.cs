using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Text;

namespace Blockify
{
    public partial class Form1 : Form
    {
        private readonly string spotifyPath = Environment.GetEnvironmentVariable("APPDATA") + @"\Spotify\spotify.exe";
        public Form1()
        {
            InitializeComponent();
        }
        public string version = "1.0.0.9"; // The version of blockify to compare with the current version on my server. If you're modifying Blockify, change this to "MOD"

            public const int KEYEVENTF_EXTENTEDKEY = 1;
            public const int KEYEVENTF_KEYUP = 0;
            public const int VK_MEDIA_NEXT_TRACK = 0xB0;
            public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
            public const int VK_MEDIA_PREV_TRACK = 0xB1;

            [DllImport("user32.dll")]
            public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        private IntPtr brwin;

        int skipAd = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            skipAd = 1;
        }

        int i = 1;
        int lpuid = 0;
        int dbg = 0;
        int dbgo = 1;
        private void timer2_Tick_1(object sender, EventArgs e)
        {
            brwin = GetForegroundWindow();
            Process[] processlist = Process.GetProcesses();
            Color darkRed = Color.FromName("DarkRed");
            if (dbgo==2)
            {
                this.Size = new Size(319, 261 + trackBar4.Value);
            }
            //Finding and setting PUID of spotify's named window. (As a later discovery, this is not the PUID. lPUID refers to what I call the "local PUID", as I'm not sure what to call this number.)
            try
            {
                if (processlist[i].MainWindowTitle.Contains("Advertisement") || processlist[i].MainWindowTitle == "Spotify")
                {
                    lpuid = i;
                    label1.ForeColor = Color.FromArgb(255, 255 / dbgo, 255 / dbgo, 255);
                } else if (processlist[i].MainWindowTitle.Contains("Spotify Free"))
                {
                    lpuid = i;
                    label1.ForeColor = Color.FromArgb(255, 255 / dbgo, 255 / dbgo, 255);
                } else if (processlist[i].MainWindowTitle.Contains(" - ") && !processlist[i].MainWindowTitle.Contains("Discord") && !processlist[i].MainWindowTitle.Contains("Microsoft Visual Studio") && !processlist[i].MainWindowTitle.Contains("Google Chrome") && !processlist[i].MainWindowTitle.Contains("Inbox - Sympatico") && !processlist[i].MainWindowTitle.Contains(" - Notepad") && !processlist[i].MainWindowTitle.Contains(" - Singleplayer"))
                {
                    lpuid = i;
                    label1.ForeColor = Color.FromArgb(255, 255 / dbgo, 255 / dbgo, 255);
                }
                if (processlist[lpuid].MainWindowTitle.Contains("Advertisement") || processlist[i].MainWindowTitle == "Spotify" || skipAd == 1) {
                    skipAd = 0;
                    label1.ForeColor = darkRed;
                    label1.Text = "Advertisement";
                    ////Skipping Ad////
                        foreach (Process proc in Process.GetProcessesByName("Spotify"))
                        {
                            brwin = GetForegroundWindow();
                            label1.ForeColor = darkRed;
                            proc.CloseMainWindow();
                            Thread.Sleep(trackBar3.Value); // Kill buffer
                            proc.Kill();
                            proc.WaitForExit();
                            SetForegroundWindow(brwin);
                        }
                        //Restarting Spotify
                        Thread.Sleep(trackBar1.Value); // Restart buffer
                        using (Process myProcess = new Process())
                        {
                        myProcess.StartInfo.FileName = spotifyPath;
                        myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                        myProcess.Start();
                        SetForegroundWindow(brwin);
                        Thread.Sleep(100);
                        }

                    Thread.Sleep(trackBar2.Value);
                    keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero); // Unpause Spotify
                    SetForegroundWindow(brwin); // Attempt to return focus to the user's previous window.
                ////End of Skipping Ad////
            }
            else if(processlist[lpuid].MainWindowTitle.Contains("Spotify Free")){
                label1.Text = "Paused";
            }else if (processlist[lpuid].MainWindowTitle.Contains(" - ") && !processlist[i].MainWindowTitle.Contains("Discord") && !processlist[i].MainWindowTitle.Contains("Microsoft Visual Studio") && !processlist[i].MainWindowTitle.Contains("Google Chrome") && !processlist[i].MainWindowTitle.Contains("Inbox - Sympatico"))
            {
                label1.Text = processlist[lpuid].MainWindowTitle;
            }
            }
            catch (Exception exc) {
                if (dbgo == 2){
                    if (!checkBox2.Checked)
                    {
                        label5.Text = "\n\n" + System.DateTime.Now + ": " + Convert.ToString(exc) + label5.Text;
                    }
                }
                if (Convert.ToString(exc).Contains("System.Diagnostics.Process.Kill"))
                {
                    using (Process myProcess = new Process())
                    {
                        label5.Text = "\n\n" + System.DateTime.Now + ": " + "Remedying kill error" + label5.Text;
                        myProcess.StartInfo.FileName = spotifyPath;
                        myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                        myProcess.Start();
                        SetForegroundWindow(brwin); // Attempt to return focus to the user's previous window again.
                        Thread.Sleep(trackBar2.Value);
                        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero); // Unpause Spotify
                        SetForegroundWindow(brwin); // Attempt to return focus to the user's previous window again.
                    }
                }
            }
            if (i > processlist.Length)
            {
                i = 1;
            }
            i = i + 1;

            //Trackbar value feedback
            label3.Text = "P Buffer: " + Convert.ToString(trackBar2.Value);
            label4.Text = "K Buffer: " + Convert.ToString(trackBar3.Value);
            label2.Text = "R Buffer: " + Convert.ToString(trackBar1.Value);

            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }

        }

            private void button2_Click(object sender, EventArgs e) // Info Button
        {
            Form form2 = new Form2();
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.ShowDialog();
        }

        ///###############///
        ///Saving Settings///
        ///###############///
        private void Form1_Load(object sender, EventArgs e)
        {
            GetSettings(); // Get settings on startup
            CheckUpdate(); // Check for an update from server
            this.Size = new Size(319, 161);
        }

        public void GetSettings() // Get settings of each value
        {
            try
            {
                trackBar3.Value = Properties.Settings.Default.KBfr;
                trackBar2.Value = Properties.Settings.Default.PBfr;
                trackBar1.Value = Properties.Settings.Default.RBfr;
            }
            catch { }
        }
        public void CheckUpdate() // Get settings of each value
        {
            try
            {

                //var client = new WebClient();
                //client.Proxy = null;
                //string url = "http://192.168.2.200/";

                //string url = "http://192.168.2.200/"; This is here for testing the program on my network.
                //http://blockify.mooo.com/ This is also here for testing on my own network, just to copy and paste quickly.
                //string content = client.DownloadString(url);

                string urlAddress = "http://blockify.mooo.com/";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                request.Timeout = 1000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (String.IsNullOrWhiteSpace(response.CharacterSet))
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                    string data = readStream.ReadToEnd();
                    if (!data.Contains(version) && version != "MOD")
                    {
                        Form form3 = new Form3();
                        form3.StartPosition = FormStartPosition.CenterParent;
                        form3.ShowDialog();

                    }

                    response.Close();
                    readStream.Close();
                }
                //!content.Contains(version) && 
            }
            catch (Exception exc2)
            {
                label1.Text = Convert.ToString(exc2);
            }
        }
        public void SaveSettings() // Save settings of each value
        {
            Properties.Settings.Default.KBfr = trackBar3.Value;
            Properties.Settings.Default.PBfr = trackBar2.Value;
            Properties.Settings.Default.RBfr = trackBar1.Value;
            Properties.Settings.Default.Save();
        }
        private void button3_Click(object sender, EventArgs e) // Run "SaveSettings" when Save button is pressed.
        {
            SaveSettings();
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
        public void label1_Click(object sender, EventArgs e)
        {
            dbg++;
            if (dbg > 2) {
                dbgo = 2;
                this.Size = new Size(319, 261+trackBar4.Value);
                this.MaximizeBox = true;
            }
        }
    }
}

//TODO
// Focus issues
// Mute / Block ads button
// Fix false detections: Add more &&!'s, keep them specific as to not call random songs Paused. // In progress
// Spotify closes then doesn't open
// Only unpause songs if audio is not playing after P buffer is over. This is because sometimes the song will not be paused upon restart, and when it is meant to be unpaused it is instead paused.