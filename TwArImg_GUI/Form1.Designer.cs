namespace TwArImg_GUI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.picBox_Guide = new System.Windows.Forms.PictureBox();
            this.lbl_Guide = new System.Windows.Forms.Label();
            this.tabCtrl_Status = new System.Windows.Forms.TabControl();
            this.tabPage_FailedTweets = new System.Windows.Forms.TabPage();
            this.lbl_FailedTweetGuide = new System.Windows.Forms.Label();
            this.list_FailedTweets = new System.Windows.Forms.ListView();
            this.ch_num = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_statusId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_authorScreenName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_failedType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage_Log = new System.Windows.Forms.TabPage();
            this.lbl_LogNotAvailable = new System.Windows.Forms.Label();
            this.txtbx_Log = new System.Windows.Forms.TextBox();
            this.list_Log = new System.Windows.Forms.ListBox();
            this.ckb_LogAutoScroll = new System.Windows.Forms.CheckBox();
            this.btn_LogSave = new System.Windows.Forms.Button();
            this.tabPage_option = new System.Windows.Forms.TabPage();
            this.lbl_Option_Login = new System.Windows.Forms.Label();
            this.ckb_Option_Login = new System.Windows.Forms.CheckBox();
            this.lbl_Option_Description = new System.Windows.Forms.Label();
            this.lbl_Option_ExcludeRetweets = new System.Windows.Forms.Label();
            this.ckb_Option_ExcludeRetweets = new System.Windows.Forms.CheckBox();
            this.tabPage_about = new System.Windows.Forms.TabPage();
            this.lnkLbl_Github = new System.Windows.Forms.LinkLabel();
            this.lbl_AboutVersion = new System.Windows.Forms.Label();
            this.lbl_AboutFFMPEG = new System.Windows.Forms.Label();
            this.lbl_AboutNewtonJson = new System.Windows.Forms.Label();
            this.lnkLbl_Twitter = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.sfd_Log = new System.Windows.Forms.SaveFileDialog();
            this.timer_LogRefresh = new System.Windows.Forms.Timer(this.components);
            this.btnWeb = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_Guide)).BeginInit();
            this.tabCtrl_Status.SuspendLayout();
            this.tabPage_FailedTweets.SuspendLayout();
            this.tabPage_Log.SuspendLayout();
            this.tabPage_option.SuspendLayout();
            this.tabPage_about.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.picBox_Guide);
            this.panel1.Controls.Add(this.lbl_Guide);
            this.panel1.Controls.Add(this.tabCtrl_Status);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // picBox_Guide
            // 
            resources.ApplyResources(this.picBox_Guide, "picBox_Guide");
            this.picBox_Guide.Image = global::TwArImg_GUI.Properties.Resources.img_main_begin_normal;
            this.picBox_Guide.Name = "picBox_Guide";
            this.picBox_Guide.TabStop = false;
            // 
            // lbl_Guide
            // 
            resources.ApplyResources(this.lbl_Guide, "lbl_Guide");
            this.lbl_Guide.Name = "lbl_Guide";
            this.lbl_Guide.UseMnemonic = false;
            // 
            // tabCtrl_Status
            // 
            resources.ApplyResources(this.tabCtrl_Status, "tabCtrl_Status");
            this.tabCtrl_Status.Controls.Add(this.tabPage_FailedTweets);
            this.tabCtrl_Status.Controls.Add(this.tabPage_Log);
            this.tabCtrl_Status.Controls.Add(this.tabPage_option);
            this.tabCtrl_Status.Controls.Add(this.tabPage_about);
            this.tabCtrl_Status.Name = "tabCtrl_Status";
            this.tabCtrl_Status.SelectedIndex = 0;
            // 
            // tabPage_FailedTweets
            // 
            this.tabPage_FailedTweets.Controls.Add(this.lbl_FailedTweetGuide);
            this.tabPage_FailedTweets.Controls.Add(this.list_FailedTweets);
            resources.ApplyResources(this.tabPage_FailedTweets, "tabPage_FailedTweets");
            this.tabPage_FailedTweets.Name = "tabPage_FailedTweets";
            this.tabPage_FailedTweets.UseVisualStyleBackColor = true;
            // 
            // lbl_FailedTweetGuide
            // 
            resources.ApplyResources(this.lbl_FailedTweetGuide, "lbl_FailedTweetGuide");
            this.lbl_FailedTweetGuide.Name = "lbl_FailedTweetGuide";
            // 
            // list_FailedTweets
            // 
            this.list_FailedTweets.AllowColumnReorder = true;
            resources.ApplyResources(this.list_FailedTweets, "list_FailedTweets");
            this.list_FailedTweets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ch_num,
            this.ch_statusId,
            this.ch_authorScreenName,
            this.ch_failedType});
            this.list_FailedTweets.FullRowSelect = true;
            this.list_FailedTweets.GridLines = true;
            this.list_FailedTweets.HideSelection = false;
            this.list_FailedTweets.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items2"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items3"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items4"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items5"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items6"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items7"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items8"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("list_FailedTweets.Items9")))});
            this.list_FailedTweets.MultiSelect = false;
            this.list_FailedTweets.Name = "list_FailedTweets";
            this.list_FailedTweets.UseCompatibleStateImageBehavior = false;
            this.list_FailedTweets.View = System.Windows.Forms.View.Details;
            this.list_FailedTweets.DoubleClick += new System.EventHandler(this.list_FailedTweets_DoubleClick);
            // 
            // ch_num
            // 
            resources.ApplyResources(this.ch_num, "ch_num");
            // 
            // ch_statusId
            // 
            resources.ApplyResources(this.ch_statusId, "ch_statusId");
            // 
            // ch_authorScreenName
            // 
            resources.ApplyResources(this.ch_authorScreenName, "ch_authorScreenName");
            // 
            // ch_failedType
            // 
            resources.ApplyResources(this.ch_failedType, "ch_failedType");
            // 
            // tabPage_Log
            // 
            this.tabPage_Log.Controls.Add(this.lbl_LogNotAvailable);
            this.tabPage_Log.Controls.Add(this.txtbx_Log);
            this.tabPage_Log.Controls.Add(this.list_Log);
            this.tabPage_Log.Controls.Add(this.ckb_LogAutoScroll);
            this.tabPage_Log.Controls.Add(this.btn_LogSave);
            resources.ApplyResources(this.tabPage_Log, "tabPage_Log");
            this.tabPage_Log.Name = "tabPage_Log";
            this.tabPage_Log.UseVisualStyleBackColor = true;
            // 
            // lbl_LogNotAvailable
            // 
            resources.ApplyResources(this.lbl_LogNotAvailable, "lbl_LogNotAvailable");
            this.lbl_LogNotAvailable.Name = "lbl_LogNotAvailable";
            // 
            // txtbx_Log
            // 
            resources.ApplyResources(this.txtbx_Log, "txtbx_Log");
            this.txtbx_Log.Name = "txtbx_Log";
            this.txtbx_Log.ReadOnly = true;
            // 
            // list_Log
            // 
            resources.ApplyResources(this.list_Log, "list_Log");
            this.list_Log.FormattingEnabled = true;
            this.list_Log.Name = "list_Log";
            // 
            // ckb_LogAutoScroll
            // 
            resources.ApplyResources(this.ckb_LogAutoScroll, "ckb_LogAutoScroll");
            this.ckb_LogAutoScroll.Checked = true;
            this.ckb_LogAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckb_LogAutoScroll.Name = "ckb_LogAutoScroll";
            this.ckb_LogAutoScroll.UseVisualStyleBackColor = true;
            // 
            // btn_LogSave
            // 
            resources.ApplyResources(this.btn_LogSave, "btn_LogSave");
            this.btn_LogSave.Name = "btn_LogSave";
            this.btn_LogSave.UseVisualStyleBackColor = true;
            // 
            // tabPage_option
            // 
            this.tabPage_option.Controls.Add(this.btnWeb);
            this.tabPage_option.Controls.Add(this.lbl_Option_Login);
            this.tabPage_option.Controls.Add(this.ckb_Option_Login);
            this.tabPage_option.Controls.Add(this.lbl_Option_Description);
            this.tabPage_option.Controls.Add(this.lbl_Option_ExcludeRetweets);
            this.tabPage_option.Controls.Add(this.ckb_Option_ExcludeRetweets);
            resources.ApplyResources(this.tabPage_option, "tabPage_option");
            this.tabPage_option.Name = "tabPage_option";
            this.tabPage_option.UseVisualStyleBackColor = true;
            // 
            // lbl_Option_Login
            // 
            resources.ApplyResources(this.lbl_Option_Login, "lbl_Option_Login");
            this.lbl_Option_Login.Name = "lbl_Option_Login";
            // 
            // ckb_Option_Login
            // 
            resources.ApplyResources(this.ckb_Option_Login, "ckb_Option_Login");
            this.ckb_Option_Login.Name = "ckb_Option_Login";
            this.ckb_Option_Login.UseVisualStyleBackColor = true;
            this.ckb_Option_Login.CheckedChanged += new System.EventHandler(this.ckb_Option_Login_CheckedChanged);
            this.ckb_Option_Login.Click += new System.EventHandler(this.ckb_Option_Login_Click);
            // 
            // lbl_Option_Description
            // 
            resources.ApplyResources(this.lbl_Option_Description, "lbl_Option_Description");
            this.lbl_Option_Description.Name = "lbl_Option_Description";
            // 
            // lbl_Option_ExcludeRetweets
            // 
            resources.ApplyResources(this.lbl_Option_ExcludeRetweets, "lbl_Option_ExcludeRetweets");
            this.lbl_Option_ExcludeRetweets.Name = "lbl_Option_ExcludeRetweets";
            // 
            // ckb_Option_ExcludeRetweets
            // 
            resources.ApplyResources(this.ckb_Option_ExcludeRetweets, "ckb_Option_ExcludeRetweets");
            this.ckb_Option_ExcludeRetweets.Name = "ckb_Option_ExcludeRetweets";
            this.ckb_Option_ExcludeRetweets.UseVisualStyleBackColor = true;
            // 
            // tabPage_about
            // 
            this.tabPage_about.Controls.Add(this.lnkLbl_Github);
            this.tabPage_about.Controls.Add(this.lbl_AboutVersion);
            this.tabPage_about.Controls.Add(this.lbl_AboutFFMPEG);
            this.tabPage_about.Controls.Add(this.lbl_AboutNewtonJson);
            this.tabPage_about.Controls.Add(this.lnkLbl_Twitter);
            this.tabPage_about.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.tabPage_about, "tabPage_about");
            this.tabPage_about.Name = "tabPage_about";
            this.tabPage_about.UseVisualStyleBackColor = true;
            // 
            // lnkLbl_Github
            // 
            resources.ApplyResources(this.lnkLbl_Github, "lnkLbl_Github");
            this.lnkLbl_Github.Name = "lnkLbl_Github";
            this.lnkLbl_Github.TabStop = true;
            this.lnkLbl_Github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLbl_Github_LinkClicked);
            // 
            // lbl_AboutVersion
            // 
            resources.ApplyResources(this.lbl_AboutVersion, "lbl_AboutVersion");
            this.lbl_AboutVersion.Name = "lbl_AboutVersion";
            // 
            // lbl_AboutFFMPEG
            // 
            resources.ApplyResources(this.lbl_AboutFFMPEG, "lbl_AboutFFMPEG");
            this.lbl_AboutFFMPEG.Name = "lbl_AboutFFMPEG";
            // 
            // lbl_AboutNewtonJson
            // 
            resources.ApplyResources(this.lbl_AboutNewtonJson, "lbl_AboutNewtonJson");
            this.lbl_AboutNewtonJson.Name = "lbl_AboutNewtonJson";
            // 
            // lnkLbl_Twitter
            // 
            resources.ApplyResources(this.lnkLbl_Twitter, "lnkLbl_Twitter");
            this.lnkLbl_Twitter.Name = "lnkLbl_Twitter";
            this.lnkLbl_Twitter.TabStop = true;
            this.lnkLbl_Twitter.UseCompatibleTextRendering = true;
            this.lnkLbl_Twitter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLbl_LaruYan_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TwArImg_GUI.Properties.Resources.img_twitter_logo;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // sfd_Log
            // 
            this.sfd_Log.DefaultExt = "log";
            this.sfd_Log.FileName = "TweetMediaArchiver";
            resources.ApplyResources(this.sfd_Log, "sfd_Log");
            // 
            // timer_LogRefresh
            // 
            this.timer_LogRefresh.Interval = 1000;
            // 
            // btnWeb
            // 
            resources.ApplyResources(this.btnWeb, "btnWeb");
            this.btnWeb.Name = "btnWeb";
            this.btnWeb.UseVisualStyleBackColor = true;
            this.btnWeb.Click += new System.EventHandler(this.btnWeb_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.DragLeave += new System.EventHandler(this.MainForm_DragLeave);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBox_Guide)).EndInit();
            this.tabCtrl_Status.ResumeLayout(false);
            this.tabPage_FailedTweets.ResumeLayout(false);
            this.tabPage_Log.ResumeLayout(false);
            this.tabPage_Log.PerformLayout();
            this.tabPage_option.ResumeLayout(false);
            this.tabPage_option.PerformLayout();
            this.tabPage_about.ResumeLayout(false);
            this.tabPage_about.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picBox_Guide;
        private System.Windows.Forms.Label lbl_Guide;
        private System.Windows.Forms.TabControl tabCtrl_Status;
        private System.Windows.Forms.TabPage tabPage_FailedTweets;
        private System.Windows.Forms.Label lbl_FailedTweetGuide;
        private System.Windows.Forms.ListView list_FailedTweets;
        private System.Windows.Forms.ColumnHeader ch_num;
        private System.Windows.Forms.ColumnHeader ch_statusId;
        private System.Windows.Forms.ColumnHeader ch_authorScreenName;
        private System.Windows.Forms.ColumnHeader ch_failedType;
        private System.Windows.Forms.TabPage tabPage_Log;
        private System.Windows.Forms.Label lbl_LogNotAvailable;
        private System.Windows.Forms.TextBox txtbx_Log;
        private System.Windows.Forms.ListBox list_Log;
        private System.Windows.Forms.CheckBox ckb_LogAutoScroll;
        private System.Windows.Forms.Button btn_LogSave;
        private System.Windows.Forms.TabPage tabPage_about;
        private System.Windows.Forms.Label lbl_AboutFFMPEG;
        private System.Windows.Forms.Label lbl_AboutNewtonJson;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel lnkLbl_Twitter;
        private System.Windows.Forms.SaveFileDialog sfd_Log;
        private System.Windows.Forms.Timer timer_LogRefresh;
        private System.Windows.Forms.Label lbl_AboutVersion;
        private System.Windows.Forms.LinkLabel lnkLbl_Github;
        private System.Windows.Forms.TabPage tabPage_option;
        private System.Windows.Forms.Label lbl_Option_ExcludeRetweets;
        private System.Windows.Forms.CheckBox ckb_Option_ExcludeRetweets;
        private System.Windows.Forms.Label lbl_Option_Description;
        private System.Windows.Forms.CheckBox ckb_Option_Login;
        private System.Windows.Forms.Label lbl_Option_Login;
        private System.Windows.Forms.Button btnWeb;
    }
}

