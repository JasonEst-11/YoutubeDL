using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplodeDemo
{
    public partial class Form1 : Form
    {
        public YoutubeClient youtube = new YoutubeClient();
        public string DlPath = "C:\\Users\\esteb\\Music"; //Default
        public FolderBrowserDialog BrowserDialog;

        public ToolTip ttip = new ToolTip();

        public Video LoadedVideo;
        public Form1()
        {
            InitializeComponent();
            textboxPath.Text = DlPath;
            SetTooltips();
        }

        private void SetTooltips()
        {            
            ttip.SetToolTip(lblTitle, lblTitle.Text);
            ttip.SetToolTip(lblAuthor, lblAuthor.Text);
            ttip.SetToolTip(lblDuration, lblDuration.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetData();            
        }

        private async void GetData() //Get Data
        {
            try
            {
                LogMessage("Fetching Data from URL...");
                LoadedVideo = await youtube.Videos.GetAsync(textboxURL.Text);
                LogMessage("Data fetched successfully!");
                lblTitle.Text = LoadedVideo.Title;
                lblAuthor.Text = LoadedVideo.Author.ChannelTitle;
                lblDuration.Text = LoadedVideo.Duration.ToString();
                SetTooltips();
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }

        }
        private void btnFileExplorer_Click(object sender, EventArgs e) //Folder Browser Dialog
        {
            BrowserDialog = new FolderBrowserDialog();
            DialogResult result = BrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {                
                textboxPath.Text = BrowserDialog.SelectedPath;
            }
        }

        private async void button2_Click(object sender, EventArgs e) //Get Audio
        {
            if (LoadedVideo != null)
            {
                try
                {
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(textboxURL.Text);
                    var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    LogMessage($"Downloading Audio: {lblTitle.Text}...");
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{textboxPath.Text}\\{lblTitle.Text}.mp3");
                    LogMessage("Download Complete!");
                }
                catch (Exception ex)
                {
                    LogMessage(ex.Message);
                }
            }
            else
            {
                LogMessage("No video loaded");
            }
        }

        private async void button3_Click(object sender, EventArgs e) //Get Video
        {
            if (LoadedVideo != null)
            {
                try
                {
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(textboxURL.Text);
                    var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                    LogMessage($"Downloading Video: {lblTitle.Text}...");
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{textboxPath.Text}\\{lblTitle.Text}.{streamInfo.Container}");
                    LogMessage("Download Complete!");
                }
                catch (Exception ex)
                {
                    LogMessage(ex.Message);
                }
            }
            else
            {
                LogMessage("No video loaded.");
            }

        }

        private void LogMessage(string message)
        {
            logBox.Text += $"{DateTime.Now.ToString()}: {message} {Environment.NewLine}";
        }

        private void textboxURL_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                GetData();
            }
        }
    }
}
