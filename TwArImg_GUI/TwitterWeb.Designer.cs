namespace TwArImg_GUI
{
    partial class TwitterWeb
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.btn_Login_CheckLogin = new System.Windows.Forms.Button();
            this.lbl_Login_Instruction = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(12, 75);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1340, 531);
            this.webBrowser1.TabIndex = 0;
            // 
            // btn_Login_CheckLogin
            // 
            this.btn_Login_CheckLogin.Location = new System.Drawing.Point(9, 12);
            this.btn_Login_CheckLogin.Name = "btn_Login_CheckLogin";
            this.btn_Login_CheckLogin.Size = new System.Drawing.Size(194, 35);
            this.btn_Login_CheckLogin.TabIndex = 1;
            this.btn_Login_CheckLogin.Text = "[HARDCODEDSTR]로그인 확인";
            this.btn_Login_CheckLogin.UseVisualStyleBackColor = true;
            this.btn_Login_CheckLogin.Click += new System.EventHandler(this.btnCheckLogin_Click);
            // 
            // lbl_Login_Instruction
            // 
            this.lbl_Login_Instruction.AutoSize = true;
            this.lbl_Login_Instruction.Location = new System.Drawing.Point(209, 17);
            this.lbl_Login_Instruction.Name = "lbl_Login_Instruction";
            this.lbl_Login_Instruction.Size = new System.Drawing.Size(982, 25);
            this.lbl_Login_Instruction.TabIndex = 2;
            this.lbl_Login_Instruction.Text = "[HARDCODEDSTR]아래 화면에서 트위터에 아카이브를 만들었던 계정으로 로그인하신 다음, 로그인 확인 버튼을 눌러주십시오.";
            // 
            // TwitterWeb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1407, 646);
            this.Controls.Add(this.lbl_Login_Instruction);
            this.Controls.Add(this.btn_Login_CheckLogin);
            this.Controls.Add(this.webBrowser1);
            this.Name = "TwitterWeb";
            this.Text = "Twitter Web";
            this.Load += new System.EventHandler(this.TwitterWeb_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button btn_Login_CheckLogin;
        private System.Windows.Forms.Label lbl_Login_Instruction;
    }
}