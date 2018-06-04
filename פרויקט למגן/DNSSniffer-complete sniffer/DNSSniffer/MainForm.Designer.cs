namespace DNSSniffer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SniffButton = new System.Windows.Forms.Button();
            this.settingsbtn = new System.Windows.Forms.Button();
            this.searchbox = new System.Windows.Forms.TextBox();
            this.browser = new System.Windows.Forms.WebBrowser();
            this.BackButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SniffButton
            // 
            this.SniffButton.Location = new System.Drawing.Point(240, 189);
            this.SniffButton.Name = "SniffButton";
            this.SniffButton.Size = new System.Drawing.Size(113, 39);
            this.SniffButton.TabIndex = 4;
            this.SniffButton.Text = "Browse Safely";
            this.SniffButton.UseVisualStyleBackColor = true;
            this.SniffButton.Click += new System.EventHandler(this.SniffButton_Click);
            // 
            // settingsbtn
            // 
            this.settingsbtn.Location = new System.Drawing.Point(12, 12);
            this.settingsbtn.Name = "settingsbtn";
            this.settingsbtn.Size = new System.Drawing.Size(56, 22);
            this.settingsbtn.TabIndex = 9;
            this.settingsbtn.Text = "Settings";
            this.settingsbtn.UseVisualStyleBackColor = true;
            this.settingsbtn.Click += new System.EventHandler(this.settingsbtn_Click);
            // 
            // searchbox
            // 
            this.searchbox.Font = new System.Drawing.Font("Open Sans", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchbox.Location = new System.Drawing.Point(114, 47);
            this.searchbox.Name = "searchbox";
            this.searchbox.Size = new System.Drawing.Size(429, 35);
            this.searchbox.TabIndex = 10;
            this.searchbox.TextChanged += new System.EventHandler(this.searchbox_TextChanged);
            // 
            // browser
            // 
            this.browser.Location = new System.Drawing.Point(15, 255);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(605, 225);
            this.browser.TabIndex = 11;
            this.browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.browser_DocumentCompleted);
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(461, 222);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(63, 23);
            this.BackButton.TabIndex = 12;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(530, 222);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 23);
            this.NextButton.TabIndex = 13;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 501);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.browser);
            this.Controls.Add(this.searchbox);
            this.Controls.Add(this.settingsbtn);
            this.Controls.Add(this.SniffButton);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SniffButton;
        private System.Windows.Forms.Button settingsbtn;
        private System.Windows.Forms.TextBox searchbox;
        private System.Windows.Forms.WebBrowser browser;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button NextButton;
    }
}

