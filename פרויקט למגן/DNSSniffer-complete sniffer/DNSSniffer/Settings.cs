using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNSSniffer
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            SetComboBox();
            SetTextBox();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //apply the changes in settings
            ApplyChanges();
            //return a positive dialog result to parent form
            this.DialogResult = DialogResult.OK;
            //after changes applied close the settings window
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void SetComboBox()
        {
            if (MainForm.savelog)
                comboBox1.SelectedIndex = 0;
            else
                comboBox1.SelectedIndex = 1;
        }
        public void SetTextBox()
        {
            pathtext.Text = MainForm.logpath;
        }

        public void ApplyChanges()
        {
            //apply changes to log path
            MainForm.logpath = MainForm.GetUniqueFilePath(pathtext.Text);
            //apply changes to log enabled or disabled
            if (comboBox1.SelectedIndex == 0)
                MainForm.savelog = true;
            if (comboBox1.SelectedIndex == 1)
                MainForm.savelog = false;
        }

        private void logpathbtn_Click(object sender, EventArgs e)
        {
            DialogResult result = logfolderdialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                pathtext.Text = logfolderdialog.SelectedPath + @"\log.txt";
            }

        }
    }
}