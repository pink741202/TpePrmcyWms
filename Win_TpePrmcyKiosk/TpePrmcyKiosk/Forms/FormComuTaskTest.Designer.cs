
namespace TpePrmcyKiosk
{
    partial class FormComuTaskTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormComuTaskTest));
            pnl_Drawers = new Panel();
            lb_CbntName = new Label();
            pic_MedicineCounter = new PictureBox();
            pnl_WaitLine = new Panel();
            ((System.ComponentModel.ISupportInitialize)pic_MedicineCounter).BeginInit();
            SuspendLayout();
            // 
            // pnl_Drawers
            // 
            pnl_Drawers.BackColor = SystemColors.InactiveCaption;
            pnl_Drawers.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point);
            pnl_Drawers.Location = new Point(34, 90);
            pnl_Drawers.Name = "pnl_Drawers";
            pnl_Drawers.Padding = new Padding(5);
            pnl_Drawers.Size = new Size(696, 367);
            pnl_Drawers.TabIndex = 0;
            // 
            // lb_CbntName
            // 
            lb_CbntName.AutoSize = true;
            lb_CbntName.Font = new Font("微軟正黑體", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb_CbntName.Location = new Point(34, 33);
            lb_CbntName.Name = "lb_CbntName";
            lb_CbntName.Size = new Size(92, 34);
            lb_CbntName.TabIndex = 1;
            lb_CbntName.Text = "label1";
            // 
            // pic_MedicineCounter
            // 
            pic_MedicineCounter.Image = (Image)resources.GetObject("pic_MedicineCounter.Image");
            pic_MedicineCounter.Location = new Point(746, 90);
            pic_MedicineCounter.Name = "pic_MedicineCounter";
            pic_MedicineCounter.Size = new Size(461, 266);
            pic_MedicineCounter.TabIndex = 2;
            pic_MedicineCounter.TabStop = false;
            // 
            // pnl_WaitLine
            // 
            pnl_WaitLine.BackColor = SystemColors.Info;
            pnl_WaitLine.Location = new Point(839, 373);
            pnl_WaitLine.Name = "pnl_WaitLine";
            pnl_WaitLine.Size = new Size(284, 327);
            pnl_WaitLine.TabIndex = 3;
            // 
            // FormComuTaskTest
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1351, 857);
            Controls.Add(pnl_WaitLine);
            Controls.Add(pic_MedicineCounter);
            Controls.Add(lb_CbntName);
            Controls.Add(pnl_Drawers);
            Font = new Font("微軟正黑體", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(5);
            Name = "FormComuTaskTest";
            Text = "FormComuTaskTest";
            FormClosing += FormComuTaskTest_FormClosing;
            Load += FormComuTaskTest_Load;
            ((System.ComponentModel.ISupportInitialize)pic_MedicineCounter).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnl_Drawers;
        private Label lb_CbntName;
        private PictureBox pic_MedicineCounter;
        private Panel pnl_WaitLine;
    }
}