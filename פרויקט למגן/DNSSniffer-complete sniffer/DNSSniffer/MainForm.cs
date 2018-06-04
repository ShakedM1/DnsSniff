using System;
using System.Collections.Generic;
using System.Drawing;
using PcapDotNet.Core;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace DNSSniffer
{

    public partial class MainForm : Form
    {
        public static bool savelog = true;       
        public bool browsing = false;
        public bool ForwardOrBack = false;
        public static string logpath = GetUniqueFilePath(@"C:\log.txt");
        public MainForm()
        {
            InitializeComponent();
            TrainModel();                     
            //handling browser events
            searchbox.KeyDown += Searchbox_KeyDown;
            browser.Navigating += Browser_Navigating;
            browser.Navigated += Browser_Navigated;
            //browser settings for startup
            browser.ScriptErrorsSuppressed = true;
            browser.Visible = false;
            UpdateButtons();
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            UpdateButtons();
        }//when new doc loaded update buttons

        public static bool ValidForCheck(string url)
        {
            //check if the url is redundant(about:blank or back/forward) or is google redirect or search
            bool isgooglesearch = url.StartsWith("https://www.google.com/search?");
            bool isaboutblank = (url == "about:blank");
            bool isgoogleredirect = url.StartsWith("https://www.google.com/url?sa");                     
            
                return !isaboutblank && !isgooglesearch && !isgoogleredirect ;
        }//valid Check

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string checkedurl = e.Url.ToString();
            string addition = "";
            //before browser navigates check if url is malicious
            if (CheckUrl(checkedurl)&& ValidForCheck(checkedurl))
            {
                addition = " Suspicious URL";                       
                e.Cancel = true;
                MessageBox.Show("URL Suspected as suspicious");
            }
            Appendlog(logpath, DateTime.Now.TimeOfDay.ToString()+ checkedurl + addition);
        }


        private void Searchbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && searchbox.Focused)
            {
                switch (searchbox.TextLength)
                {
                    case 0:
                        MessageBox.Show("Please write something! The search bar was left empty");
                        break;

                    default:
                        if (searchbox.TextLength < 8)
                        {
                            MessageBox.Show("please enter a valid url");
                            break;
                        }
                        else
                        {
                            string result = "";
                            if (CheckUrl(searchbox.Text))
                                result = "Website Potentially Malicious";
                            else
                                result = "Website Is Safe";

                            MessageBox.Show(result);                           
                        }
                        break;
                }//switch

            }//if
        }//keydown

       
        private void SniffButton_Click(object sender, EventArgs e)
        {
            
            if(!browsing)
            { 
                //create the log file with introduction line
                if (savelog)
                {
                    string str = "Log File Created " + DateTime.Now.ToString() + "  Browsing History: ";
                    string path = GetUniqueFilePath(logpath);
                    System.IO.File.WriteAllText(path, str);
                    logpath = path;
                }
                //
                SniffButton.Text = "Stop Browsing";                
                browsing = true;
                //show the browser and buttons
                browser.Visible = true;
                browser.Navigate("www.google.com");
                BackButton.Visible = true;
                NextButton.Visible = true;
            }
            else if (browsing)
            {
                //end log
                Appendlog(logpath,"Browsing Stopped.");

                SniffButton.Text = "Browse Safely";               
                //hide browser
                browser.Visible = false;               
                BackButton.Visible = false;
                NextButton.Visible = false;
                browsing = false;
            }
        }//BrowsingClick

                   
        public static string GetUniqueFilePath(string filepath)
        {
            if (File.Exists(filepath))
            {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 1;

                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(filepath));
            }

            return filepath;
        }//getunuiquepath

        public void Appendlog(string path, string content)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(content);
            }
        }//appendlog

       
        public static bool CheckUrl(string url)
        {                        
                CSharp2Python py = new CSharp2Python();
                string path = GetScriptPath("");
                string result = py.ExecuteCommandWithReturn("cd " + path + "&" + "python -c \"import classify; classify.checkurl('" + url + "')\"");                
                if (result.Contains("1"))
                    return true;
                else
                    return false;           
        }//checkurl

        public static void TrainModel()
        {
            string str = @"..\..\TrainedModel\complete_model.sav";
            if (!File.Exists(str))
            {
                CSharp2Python py = new CSharp2Python();
                MessageBox.Show("creating model...");
                string p1 = "\"" + GetScriptPath("predicturl.py") + "\"";
                py.ExecuteScript(p1);
            }
        }//Train Model

        public static string GetScriptPath(string scriptname)
        {
            string path = Path.GetFullPath(@"..\..\Scripts\" + scriptname);
            return path;
        }

        private void settingsbtn_Click(object sender, EventArgs e)
        {
            //only allow settings change if not sniffing
            if (!browsing)
            {
                Settings settingswindow = new Settings();
                if (settingswindow.ShowDialog(this) == DialogResult.OK)
                { MessageBox.Show("Settings Applied"); }
                else { MessageBox.Show("Error:Settings did not apply properly"); }
                settingswindow.Dispose();
                //settingswindow.Show();
            }
            else
                MessageBox.Show("To access settings, stop browsing.");
        }//settingsbtn

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void searchbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (IsHorizontalScrollbarPresent)
            {
                browser.Size = new Size(browser.Document.Body.ScrollRectangle.Width, browser.Document.Body.ScrollRectangle.Height);
                MainForm.ActiveForm.Size = new Size(browser.Document.Body.ScrollRectangle.Width, browser.Document.Body.ScrollRectangle.Height);

            }
        }

        public bool IsHorizontalScrollbarPresent
        {
            get
            {
                var widthofScrollableArea = browser.Document.Body.ScrollRectangle.Width;
                var widthofControl = browser.Document.Window.Size.Width;

                return widthofScrollableArea > widthofControl;
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            browser.GoBack();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            browser.GoForward();
        }
        public void UpdateButtons()
        {
            if (browser.CanGoBack)
                BackButton.Visible = true;
            else
                BackButton.Visible = false;

            if (browser.CanGoForward)
                NextButton.Visible = true;
            else
                NextButton.Visible = false;
        }//updates the back and next page buttons       

        
    }
}

