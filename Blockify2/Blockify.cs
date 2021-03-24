using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Windows.Automation;
using System.Net;
using System.IO;
using System.Text;

namespace Start_program_without_stealing_focus_snippet
{
    public partial class Blockify : Form
    {
        ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~///
        ///Before publishing Blockify, be sure to update the "version" variable to something higher.///
        ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~///
        public string version = "1.0.1.0";


        public Blockify()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        IntPtr appWin1;
        Process spotify;

        private readonly string spotifyPath = Environment.GetEnvironmentVariable("APPDATA") + @"\Spotify\spotify.exe";
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckUpdate();
            startSpotify();
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startSpotify();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.appWin1 != IntPtr.Zero)
            {
                MoveWindow(appWin1, 0, 0, this.Width / 2, this.Height, true);
            }
            //base.OnResize(e);
        }

        public void startSpotify()
        {
            try
            {
                ProcessStartInfo ps1 = new ProcessStartInfo(spotifyPath);
                ps1.WindowStyle = ProcessWindowStyle.Minimized;
                Process p1 = Process.Start(ps1);
                Thread.Sleep(1000); // Allow the process to open it's window
                appWin1 = p1.MainWindowHandle;
                spotify = p1;
                // Put it into this form
                SetParent(appWin1, this.Handle);
                // Move the window to overlay it on this window
                MoveWindow(appWin1, 0, 70, this.Width / 2, this.Height/2, true);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error");
            }
        }
        public int tmr = 0;
        public void closeSpotify()
        {
            try
            {
                spotify.CloseMainWindow();
                Thread.Sleep(1000);
                spotify.Kill();
                tmr = 0;
            }
            catch { }
        }
        IntPtr brwin;
        public void skipAd()
        {
            closeSpotify();
            Thread.Sleep(1000);
            brwin = GetForegroundWindow();
            startSpotify();
            Thread.Sleep(80);
            SetForegroundWindow(brwin);
            Thread.Sleep(1000);
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero); // Unpause Spotify
        }

        public void CheckAd() // Check if an ad is playing
        {
            if (tmr > 100)
            {
                try
                {
                    var element = AutomationElement.FromHandle(appWin1);
                    if (element.Current.Name.Contains("Advertisement") || element.Current.Name == "Spotify")
                    {
                        label2.Text = "Advertisement";
                        skipAd();
                    }
                    else if (element.Current.Name == "Spotify Free")
                    {
                        label2.Text = "Paused";
                    }
                    else { label2.Text = element.Current.Name; }

                }
                catch
                {

                }
            }
        }
        public void CheckUpdate() // Get settings of each value
        {
            try
            {
                string urlAddress = "http://blockify.mooo.com/";
                // http://blockify.mooo.com/
                // http://192.168.2.200/

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
                        Form form3 = new Update();
                        form3.StartPosition = FormStartPosition.CenterParent;
                        form3.ShowDialog();
                    }

                    response.Close();
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckAd();
            tmr++;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            MoveWindow(appWin1, 0, 70, this.Width-20, this.Height-115, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            skipAd();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form form2 = new Info();
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.ShowDialog();
        }
    }
}