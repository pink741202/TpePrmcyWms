namespace TpePrmcyKiosk
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            menu_login = new ToolStripMenuItem();
            menu_browser = new ToolStripMenuItem();
            menu_Setup = new ToolStripMenuItem();
            menu_InitialScale = new ToolStripMenuItem();
            menu_DeviceDetect = new ToolStripMenuItem();
            menu_Test = new ToolStripMenuItem();
            menu_ComuSensors = new ToolStripMenuItem();
            menu_HsptlApi = new ToolStripMenuItem();
            menu_LEDStrip = new ToolStripMenuItem();
            menu_CloseSys = new ToolStripMenuItem();
            mainpanel = new Panel();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.AntiqueWhite;
            menuStrip1.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { menu_login, menu_browser, menu_Setup, menu_Test, menu_CloseSys });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1784, 32);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // menu_login
            // 
            menu_login.Name = "menu_login";
            menu_login.Size = new Size(60, 28);
            menu_login.Text = "首頁";
            menu_login.Click += menu_login_Click;
            // 
            // menu_browser
            // 
            menu_browser.Name = "menu_browser";
            menu_browser.Size = new Size(98, 28);
            menu_browser.Text = "操作平台";
            menu_browser.Click += menu_browser_Click;
            // 
            // menu_Setup
            // 
            menu_Setup.DropDownItems.AddRange(new ToolStripItem[] { menu_InitialScale, menu_DeviceDetect });
            menu_Setup.Name = "menu_Setup";
            menu_Setup.Size = new Size(60, 28);
            menu_Setup.Text = "設定";
            // 
            // menu_InitialScale
            // 
            menu_InitialScale.Name = "menu_InitialScale";
            menu_InitialScale.Size = new Size(213, 28);
            menu_InitialScale.Text = "磅秤初始化";
            menu_InitialScale.Click += menu_InitialScale_Click;
            // 
            // menu_DeviceDetect
            // 
            menu_DeviceDetect.Name = "menu_DeviceDetect";
            menu_DeviceDetect.Size = new Size(213, 28);
            menu_DeviceDetect.Text = "感應器設備偵測";
            menu_DeviceDetect.Click += menu_DeviceDetect_Click;
            // 
            // menu_Test
            // 
            menu_Test.DropDownItems.AddRange(new ToolStripItem[] { menu_ComuSensors, menu_HsptlApi, menu_LEDStrip });
            menu_Test.Name = "menu_Test";
            menu_Test.Size = new Size(60, 28);
            menu_Test.Text = "測試";
            // 
            // menu_ComuSensors
            // 
            menu_ComuSensors.Name = "menu_ComuSensors";
            menu_ComuSensors.Size = new Size(213, 28);
            menu_ComuSensors.Text = "感應器流程測試";
            menu_ComuSensors.Click += menu_ComuSensors_Click;
            // 
            // menu_HsptlApi
            // 
            menu_HsptlApi.Name = "menu_HsptlApi";
            menu_HsptlApi.Size = new Size(213, 28);
            menu_HsptlApi.Text = "院方API";
            menu_HsptlApi.Click += menu_HsptlApi_Click;
            // 
            // menu_LEDStrip
            // 
            menu_LEDStrip.Name = "menu_LEDStrip";
            menu_LEDStrip.Size = new Size(213, 28);
            menu_LEDStrip.Text = "LED燈條";
            menu_LEDStrip.Click += LEDToolStripMenuItem_Click;
            // 
            // menu_CloseSys
            // 
            menu_CloseSys.Name = "menu_CloseSys";
            menu_CloseSys.Size = new Size(98, 28);
            menu_CloseSys.Text = "關閉系統";
            menu_CloseSys.Click += Menu_CloseSys_Click;
            // 
            // mainpanel
            // 
            mainpanel.AutoSize = true;
            mainpanel.Dock = DockStyle.Fill;
            mainpanel.Location = new Point(0, 0);
            mainpanel.Margin = new Padding(3, 33, 3, 3);
            mainpanel.Name = "mainpanel";
            mainpanel.Padding = new Padding(0, 33, 0, 0);
            mainpanel.Size = new Size(1784, 961);
            mainpanel.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1784, 961);
            Controls.Add(menuStrip1);
            Controls.Add(mainpanel);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "主視窗";
            FormClosing += MainForm_FormClosing;
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menu_login;
        private ToolStripMenuItem menu_browser;
        private Panel mainpanel;
        private ToolStripMenuItem menu_CloseSys;
        private ToolStripMenuItem menu_Setup;
        private ToolStripMenuItem menu_InitialScale;
        private ToolStripMenuItem menu_Test;
        private ToolStripMenuItem menu_ComuSensors;
        private ToolStripMenuItem menu_HsptlApi;
        private ToolStripMenuItem menu_LEDStrip;
        private ToolStripMenuItem menu_DeviceDetect;
    }
}