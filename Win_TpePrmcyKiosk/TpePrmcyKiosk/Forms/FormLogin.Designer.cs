
namespace TpePrmcyKiosk
{
    partial class FormLogin
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
            this.lb_welcome = new System.Windows.Forms.Label();
            this.btn_CloseSys = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_welcome
            // 
            this.lb_welcome.AutoSize = true;
            this.lb_welcome.Font = new System.Drawing.Font("Microsoft JhengHei UI", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lb_welcome.Location = new System.Drawing.Point(253, 143);
            this.lb_welcome.Name = "lb_welcome";
            this.lb_welcome.Size = new System.Drawing.Size(319, 81);
            this.lb_welcome.TabIndex = 0;
            this.lb_welcome.Text = "Welcome";
            // 
            // btn_CloseSys
            // 
            this.btn_CloseSys.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_CloseSys.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_CloseSys.Font = new System.Drawing.Font("Microsoft JhengHei UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btn_CloseSys.Location = new System.Drawing.Point(270, 349);
            this.btn_CloseSys.Name = "btn_CloseSys";
            this.btn_CloseSys.Size = new System.Drawing.Size(277, 116);
            this.btn_CloseSys.TabIndex = 1;
            this.btn_CloseSys.Text = "關閉系統";
            this.btn_CloseSys.UseVisualStyleBackColor = false;
            this.btn_CloseSys.Click += new System.EventHandler(this.btn_CloseSys_Click);
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(779, 515);
            this.Controls.Add(this.btn_CloseSys);
            this.Controls.Add(this.lb_welcome);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLogin";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_welcome;
        private System.Windows.Forms.Button btn_CloseSys;
    }
}