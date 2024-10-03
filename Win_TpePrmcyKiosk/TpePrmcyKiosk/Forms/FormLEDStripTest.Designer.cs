
namespace TpePrmcyKiosk
{
    partial class FormLEDStripTest
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
            txtReciveData = new TextBox();
            txt_com = new TextBox();
            lb_com = new Label();
            txt_LEDcnt = new TextBox();
            label1 = new Label();
            txt_OnIndex = new TextBox();
            label2 = new Label();
            cb_colorpicker = new ComboBox();
            label3 = new Label();
            btn_submit = new Button();
            SuspendLayout();
            // 
            // txtReciveData
            // 
            txtReciveData.Location = new Point(64, 338);
            txtReciveData.Multiline = true;
            txtReciveData.Name = "txtReciveData";
            txtReciveData.Size = new Size(1299, 380);
            txtReciveData.TabIndex = 0;
            // 
            // txt_com
            // 
            txt_com.Font = new Font("微軟正黑體", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            txt_com.Location = new Point(126, 31);
            txt_com.Name = "txt_com";
            txt_com.Size = new Size(50, 35);
            txt_com.TabIndex = 1;
            txt_com.Text = "3";
            // 
            // lb_com
            // 
            lb_com.AutoSize = true;
            lb_com.Location = new Point(64, 34);
            lb_com.Name = "lb_com";
            lb_com.Size = new Size(56, 24);
            lb_com.TabIndex = 2;
            lb_com.Text = "COM";
            // 
            // txt_LEDcnt
            // 
            txt_LEDcnt.Font = new Font("微軟正黑體", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            txt_LEDcnt.Location = new Point(294, 31);
            txt_LEDcnt.Name = "txt_LEDcnt";
            txt_LEDcnt.Size = new Size(61, 35);
            txt_LEDcnt.TabIndex = 3;
            txt_LEDcnt.Text = "30";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(201, 39);
            label1.Name = "label1";
            label1.Size = new Size(86, 24);
            label1.TabIndex = 2;
            label1.Text = "燈粒總數";
            // 
            // txt_OnIndex
            // 
            txt_OnIndex.Font = new Font("微軟正黑體", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            txt_OnIndex.Location = new Point(65, 151);
            txt_OnIndex.Name = "txt_OnIndex";
            txt_OnIndex.Size = new Size(1299, 35);
            txt_OnIndex.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(65, 114);
            label2.Name = "label2";
            label2.Size = new Size(304, 24);
            label2.TabIndex = 2;
            label2.Text = "要亮燈的位置，例如：1,5,10,11,15";
            // 
            // cb_colorpicker
            // 
            cb_colorpicker.Font = new Font("微軟正黑體", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            cb_colorpicker.FormattingEnabled = true;
            cb_colorpicker.Items.AddRange(new object[] { "紅:red", "橙:org", "黃:ylw", "綠:grn", "藍:blu", "紫:ppl", "白:wit" });
            cb_colorpicker.Location = new Point(201, 201);
            cb_colorpicker.Name = "cb_colorpicker";
            cb_colorpicker.Size = new Size(125, 35);
            cb_colorpicker.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(64, 208);
            label3.Name = "label3";
            label3.Size = new Size(124, 24);
            label3.TabIndex = 2;
            label3.Text = "要亮燈的顏色";
            // 
            // btn_submit
            // 
            btn_submit.BackColor = SystemColors.Info;
            btn_submit.Font = new Font("微軟正黑體", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            btn_submit.Location = new Point(990, 201);
            btn_submit.Name = "btn_submit";
            btn_submit.Size = new Size(373, 86);
            btn_submit.TabIndex = 5;
            btn_submit.Text = "送出";
            btn_submit.UseVisualStyleBackColor = false;
            btn_submit.Click += btn_submit_Click;
            // 
            // FormLEDStripTest
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1471, 797);
            Controls.Add(btn_submit);
            Controls.Add(cb_colorpicker);
            Controls.Add(txt_OnIndex);
            Controls.Add(txt_LEDcnt);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(lb_com);
            Controls.Add(txt_com);
            Controls.Add(txtReciveData);
            Font = new Font("微軟正黑體", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(5);
            Name = "FormLEDStripTest";
            Text = "操作平台";
            Load += FormLEDStripTest_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtReciveData;
        private TextBox txt_com;
        private Label lb_com;
        private TextBox txt_LEDcnt;
        private Label label1;
        private TextBox txt_OnIndex;
        private Label label2;
        private ComboBox cb_colorpicker;
        private Label label3;
        private Button btn_submit;
    }
}