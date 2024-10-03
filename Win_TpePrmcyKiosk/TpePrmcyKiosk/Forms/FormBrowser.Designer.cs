
namespace TpePrmcyKiosk
{
    partial class FormBrowser
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
            wV_mainsys = new Microsoft.Web.WebView2.WinForms.WebView2();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripMsg = new ToolStripStatusLabel();
            toolStripDoorChk = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)wV_mainsys).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // wV_mainsys
            // 
            wV_mainsys.AllowExternalDrop = false;
            wV_mainsys.CreationProperties = null;
            wV_mainsys.DefaultBackgroundColor = Color.White;
            wV_mainsys.Dock = DockStyle.Fill;
            wV_mainsys.Location = new Point(0, 0);
            wV_mainsys.Name = "wV_mainsys";
            wV_mainsys.Size = new Size(1800, 952);
            wV_mainsys.TabIndex = 0;
            wV_mainsys.ZoomFactor = 1D;
            wV_mainsys.SourceChanged += wV_mainsys_SourceChanged;
            wV_mainsys.KeyDown += wV_mainsys_KeyDown;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel, toolStripMsg, toolStripDoorChk });
            statusStrip1.Location = new Point(0, 930);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1800, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(0, 17);
            // 
            // toolStripMsg
            // 
            toolStripMsg.Name = "toolStripMsg";
            toolStripMsg.Size = new Size(0, 17);
            // 
            // toolStripDoorChk
            // 
            toolStripDoorChk.Name = "toolStripDoorChk";
            toolStripDoorChk.Size = new Size(0, 17);
            // 
            // FormBrowser
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1800, 952);
            Controls.Add(statusStrip1);
            Controls.Add(wV_mainsys);
            Font = new Font("微軟正黑體", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(5);
            Name = "FormBrowser";
            Text = "操作平台";
            Load += FormBrowser_Load;
            ((System.ComponentModel.ISupportInitialize)wV_mainsys).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 wV_mainsys;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripStatusLabel toolStripMsg;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStatusDoorChk;
        private ToolStripStatusLabel toolStripDoorChk;
    }
}