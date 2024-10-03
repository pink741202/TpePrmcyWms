
namespace TpePrmcyKiosk
{
    partial class FormHsptlApiTest
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
            btn_getemp = new Button();
            tb_getemp = new TextBox();
            gb_getemp = new GroupBox();
            tb_log = new TextBox();
            gb_getdrug = new GroupBox();
            bt_getdrug = new Button();
            tb_getdrug = new TextBox();
            gb_outstore = new GroupBox();
            bt_outstore = new Button();
            tb_outstore = new TextBox();
            gb_returnqrcode = new GroupBox();
            bt_returnqrcode = new Button();
            tb_returnqrcode = new TextBox();
            gb_returnhand = new GroupBox();
            bt_returnhand = new Button();
            tb_returnhand4 = new TextBox();
            tb_returnhand3 = new TextBox();
            tb_returnhand2 = new TextBox();
            tb_returnhand1 = new TextBox();
            gb_ipdseq = new GroupBox();
            bt_ipdseq = new Button();
            tb_ipdseq2 = new TextBox();
            tb_ipdseq1 = new TextBox();
            gb_ipdrestore = new GroupBox();
            bt_ipdrestore = new Button();
            tb_ipdrestore4 = new TextBox();
            tb_ipdrestore3 = new TextBox();
            tb_ipdrestore2 = new TextBox();
            tb_ipdrestore1 = new TextBox();
            groupBox1 = new GroupBox();
            btn_inventory = new Button();
            txt_inventory = new TextBox();
            gb_getemp.SuspendLayout();
            gb_getdrug.SuspendLayout();
            gb_outstore.SuspendLayout();
            gb_returnqrcode.SuspendLayout();
            gb_returnhand.SuspendLayout();
            gb_ipdseq.SuspendLayout();
            gb_ipdrestore.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btn_getemp
            // 
            btn_getemp.Location = new Point(146, 74);
            btn_getemp.Margin = new Padding(4);
            btn_getemp.Name = "btn_getemp";
            btn_getemp.Size = new Size(95, 34);
            btn_getemp.TabIndex = 0;
            btn_getemp.Text = "送出";
            btn_getemp.UseVisualStyleBackColor = true;
            btn_getemp.Click += btn_getemp_Click;
            // 
            // tb_getemp
            // 
            tb_getemp.Location = new Point(43, 26);
            tb_getemp.Name = "tb_getemp";
            tb_getemp.PlaceholderText = "輸入員編";
            tb_getemp.Size = new Size(198, 27);
            tb_getemp.TabIndex = 2;
            // 
            // gb_getemp
            // 
            gb_getemp.BackColor = SystemColors.ActiveCaption;
            gb_getemp.Controls.Add(btn_getemp);
            gb_getemp.Controls.Add(tb_getemp);
            gb_getemp.Location = new Point(30, 36);
            gb_getemp.Name = "gb_getemp";
            gb_getemp.Size = new Size(266, 127);
            gb_getemp.TabIndex = 3;
            gb_getemp.TabStop = false;
            gb_getemp.Text = "取得員工";
            // 
            // tb_log
            // 
            tb_log.BorderStyle = BorderStyle.FixedSingle;
            tb_log.Location = new Point(904, 36);
            tb_log.Multiline = true;
            tb_log.Name = "tb_log";
            tb_log.ScrollBars = ScrollBars.Both;
            tb_log.Size = new Size(553, 828);
            tb_log.TabIndex = 4;
            // 
            // gb_getdrug
            // 
            gb_getdrug.BackColor = SystemColors.ActiveCaption;
            gb_getdrug.Controls.Add(bt_getdrug);
            gb_getdrug.Controls.Add(tb_getdrug);
            gb_getdrug.Location = new Point(30, 193);
            gb_getdrug.Name = "gb_getdrug";
            gb_getdrug.Size = new Size(266, 127);
            gb_getdrug.TabIndex = 4;
            gb_getdrug.TabStop = false;
            gb_getdrug.Text = "取得藥品";
            // 
            // bt_getdrug
            // 
            bt_getdrug.Location = new Point(146, 74);
            bt_getdrug.Margin = new Padding(4);
            bt_getdrug.Name = "bt_getdrug";
            bt_getdrug.Size = new Size(95, 34);
            bt_getdrug.TabIndex = 0;
            bt_getdrug.Text = "送出";
            bt_getdrug.UseVisualStyleBackColor = true;
            bt_getdrug.Click += bt_getdrug_Click;
            // 
            // tb_getdrug
            // 
            tb_getdrug.Location = new Point(43, 26);
            tb_getdrug.Name = "tb_getdrug";
            tb_getdrug.PlaceholderText = "輸入藥碼";
            tb_getdrug.Size = new Size(198, 27);
            tb_getdrug.TabIndex = 2;
            // 
            // gb_outstore
            // 
            gb_outstore.BackColor = Color.RosyBrown;
            gb_outstore.Controls.Add(bt_outstore);
            gb_outstore.Controls.Add(tb_outstore);
            gb_outstore.Location = new Point(317, 36);
            gb_outstore.Name = "gb_outstore";
            gb_outstore.Size = new Size(266, 127);
            gb_outstore.TabIndex = 5;
            gb_outstore.TabStop = false;
            gb_outstore.Text = "檢驗出藥";
            // 
            // bt_outstore
            // 
            bt_outstore.Location = new Point(146, 74);
            bt_outstore.Margin = new Padding(4);
            bt_outstore.Name = "bt_outstore";
            bt_outstore.Size = new Size(95, 34);
            bt_outstore.TabIndex = 0;
            bt_outstore.Text = "送出";
            bt_outstore.UseVisualStyleBackColor = true;
            bt_outstore.Click += bt_outstore_Click;
            // 
            // tb_outstore
            // 
            tb_outstore.Location = new Point(43, 26);
            tb_outstore.Name = "tb_outstore";
            tb_outstore.PlaceholderText = "輸入QRCODE";
            tb_outstore.Size = new Size(198, 27);
            tb_outstore.TabIndex = 2;
            // 
            // gb_returnqrcode
            // 
            gb_returnqrcode.BackColor = Color.SeaGreen;
            gb_returnqrcode.Controls.Add(bt_returnqrcode);
            gb_returnqrcode.Controls.Add(tb_returnqrcode);
            gb_returnqrcode.Location = new Point(317, 193);
            gb_returnqrcode.Name = "gb_returnqrcode";
            gb_returnqrcode.Size = new Size(266, 127);
            gb_returnqrcode.TabIndex = 6;
            gb_returnqrcode.TabStop = false;
            gb_returnqrcode.Text = "檢驗退藥(QRCODE)";
            // 
            // bt_returnqrcode
            // 
            bt_returnqrcode.Location = new Point(146, 74);
            bt_returnqrcode.Margin = new Padding(4);
            bt_returnqrcode.Name = "bt_returnqrcode";
            bt_returnqrcode.Size = new Size(95, 34);
            bt_returnqrcode.TabIndex = 0;
            bt_returnqrcode.Text = "送出";
            bt_returnqrcode.UseVisualStyleBackColor = true;
            bt_returnqrcode.Click += bt_returnqrcode_Click;
            // 
            // tb_returnqrcode
            // 
            tb_returnqrcode.Location = new Point(43, 26);
            tb_returnqrcode.Name = "tb_returnqrcode";
            tb_returnqrcode.PlaceholderText = "輸入QRCODE";
            tb_returnqrcode.Size = new Size(198, 27);
            tb_returnqrcode.TabIndex = 2;
            // 
            // gb_returnhand
            // 
            gb_returnhand.BackColor = Color.SeaGreen;
            gb_returnhand.Controls.Add(bt_returnhand);
            gb_returnhand.Controls.Add(tb_returnhand4);
            gb_returnhand.Controls.Add(tb_returnhand3);
            gb_returnhand.Controls.Add(tb_returnhand2);
            gb_returnhand.Controls.Add(tb_returnhand1);
            gb_returnhand.Location = new Point(317, 343);
            gb_returnhand.Name = "gb_returnhand";
            gb_returnhand.Size = new Size(266, 213);
            gb_returnhand.TabIndex = 7;
            gb_returnhand.TabStop = false;
            gb_returnhand.Text = "檢驗退藥(手動)";
            // 
            // bt_returnhand
            // 
            bt_returnhand.Location = new Point(146, 163);
            bt_returnhand.Margin = new Padding(4);
            bt_returnhand.Name = "bt_returnhand";
            bt_returnhand.Size = new Size(95, 34);
            bt_returnhand.TabIndex = 0;
            bt_returnhand.Text = "送出";
            bt_returnhand.UseVisualStyleBackColor = true;
            bt_returnhand.Click += bt_returnhand_Click;
            // 
            // tb_returnhand4
            // 
            tb_returnhand4.Location = new Point(43, 125);
            tb_returnhand4.Name = "tb_returnhand4";
            tb_returnhand4.PlaceholderText = "報告結束日期 YYYMMDD";
            tb_returnhand4.Size = new Size(198, 27);
            tb_returnhand4.TabIndex = 2;
            // 
            // tb_returnhand3
            // 
            tb_returnhand3.Location = new Point(43, 92);
            tb_returnhand3.Name = "tb_returnhand3";
            tb_returnhand3.PlaceholderText = "出藥日期 YYYMMDD";
            tb_returnhand3.Size = new Size(198, 27);
            tb_returnhand3.TabIndex = 2;
            // 
            // tb_returnhand2
            // 
            tb_returnhand2.Location = new Point(43, 59);
            tb_returnhand2.Name = "tb_returnhand2";
            tb_returnhand2.PlaceholderText = "病歷號碼";
            tb_returnhand2.Size = new Size(198, 27);
            tb_returnhand2.TabIndex = 2;
            // 
            // tb_returnhand1
            // 
            tb_returnhand1.Location = new Point(43, 26);
            tb_returnhand1.Name = "tb_returnhand1";
            tb_returnhand1.PlaceholderText = "院區代碼";
            tb_returnhand1.Size = new Size(198, 27);
            tb_returnhand1.TabIndex = 2;
            // 
            // gb_ipdseq
            // 
            gb_ipdseq.BackColor = Color.LightSeaGreen;
            gb_ipdseq.Controls.Add(bt_ipdseq);
            gb_ipdseq.Controls.Add(tb_ipdseq2);
            gb_ipdseq.Controls.Add(tb_ipdseq1);
            gb_ipdseq.Location = new Point(606, 36);
            gb_ipdseq.Name = "gb_ipdseq";
            gb_ipdseq.Size = new Size(266, 148);
            gb_ipdseq.TabIndex = 8;
            gb_ipdseq.TabStop = false;
            gb_ipdseq.Text = "取得住院病人序號";
            // 
            // bt_ipdseq
            // 
            bt_ipdseq.Location = new Point(146, 102);
            bt_ipdseq.Margin = new Padding(4);
            bt_ipdseq.Name = "bt_ipdseq";
            bt_ipdseq.Size = new Size(95, 34);
            bt_ipdseq.TabIndex = 0;
            bt_ipdseq.Text = "送出";
            bt_ipdseq.UseVisualStyleBackColor = true;
            bt_ipdseq.Click += bt_ipdseq_Click;
            // 
            // tb_ipdseq2
            // 
            tb_ipdseq2.Location = new Point(43, 59);
            tb_ipdseq2.Name = "tb_ipdseq2";
            tb_ipdseq2.PlaceholderText = "病歷號碼";
            tb_ipdseq2.Size = new Size(198, 27);
            tb_ipdseq2.TabIndex = 2;
            // 
            // tb_ipdseq1
            // 
            tb_ipdseq1.Location = new Point(43, 26);
            tb_ipdseq1.Name = "tb_ipdseq1";
            tb_ipdseq1.PlaceholderText = "院區代碼";
            tb_ipdseq1.Size = new Size(198, 27);
            tb_ipdseq1.TabIndex = 2;
            // 
            // gb_ipdrestore
            // 
            gb_ipdrestore.BackColor = Color.LightSeaGreen;
            gb_ipdrestore.Controls.Add(bt_ipdrestore);
            gb_ipdrestore.Controls.Add(tb_ipdrestore4);
            gb_ipdrestore.Controls.Add(tb_ipdrestore3);
            gb_ipdrestore.Controls.Add(tb_ipdrestore2);
            gb_ipdrestore.Controls.Add(tb_ipdrestore1);
            gb_ipdrestore.Location = new Point(606, 216);
            gb_ipdrestore.Name = "gb_ipdrestore";
            gb_ipdrestore.Size = new Size(266, 213);
            gb_ipdrestore.TabIndex = 8;
            gb_ipdrestore.TabStop = false;
            gb_ipdrestore.Text = "住院退藥";
            // 
            // bt_ipdrestore
            // 
            bt_ipdrestore.Location = new Point(146, 163);
            bt_ipdrestore.Margin = new Padding(4);
            bt_ipdrestore.Name = "bt_ipdrestore";
            bt_ipdrestore.Size = new Size(95, 34);
            bt_ipdrestore.TabIndex = 0;
            bt_ipdrestore.Text = "送出";
            bt_ipdrestore.UseVisualStyleBackColor = true;
            bt_ipdrestore.Click += bt_ipdrestore_Click;
            // 
            // tb_ipdrestore4
            // 
            tb_ipdrestore4.Location = new Point(43, 125);
            tb_ipdrestore4.Name = "tb_ipdrestore4";
            tb_ipdrestore4.PlaceholderText = "退藥單號";
            tb_ipdrestore4.Size = new Size(198, 27);
            tb_ipdrestore4.TabIndex = 2;
            // 
            // tb_ipdrestore3
            // 
            tb_ipdrestore3.Location = new Point(43, 92);
            tb_ipdrestore3.Name = "tb_ipdrestore3";
            tb_ipdrestore3.PlaceholderText = "病人序號";
            tb_ipdrestore3.Size = new Size(198, 27);
            tb_ipdrestore3.TabIndex = 2;
            // 
            // tb_ipdrestore2
            // 
            tb_ipdrestore2.Location = new Point(43, 59);
            tb_ipdrestore2.Name = "tb_ipdrestore2";
            tb_ipdrestore2.PlaceholderText = "病歷號碼";
            tb_ipdrestore2.Size = new Size(198, 27);
            tb_ipdrestore2.TabIndex = 2;
            // 
            // tb_ipdrestore1
            // 
            tb_ipdrestore1.Location = new Point(43, 26);
            tb_ipdrestore1.Name = "tb_ipdrestore1";
            tb_ipdrestore1.PlaceholderText = "院區代碼";
            tb_ipdrestore1.Size = new Size(198, 27);
            tb_ipdrestore1.TabIndex = 2;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Info;
            groupBox1.Controls.Add(btn_inventory);
            groupBox1.Controls.Add(txt_inventory);
            groupBox1.Location = new Point(606, 651);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(266, 127);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "取得庫存";
            // 
            // btn_inventory
            // 
            btn_inventory.Location = new Point(146, 74);
            btn_inventory.Margin = new Padding(4);
            btn_inventory.Name = "btn_inventory";
            btn_inventory.Size = new Size(95, 34);
            btn_inventory.TabIndex = 0;
            btn_inventory.Text = "送出";
            btn_inventory.UseVisualStyleBackColor = true;
            btn_inventory.Click += bt_getInventory_Click;
            // 
            // txt_inventory
            // 
            txt_inventory.Location = new Point(43, 26);
            txt_inventory.Name = "txt_inventory";
            txt_inventory.PlaceholderText = "輸入參數";
            txt_inventory.Size = new Size(198, 27);
            txt_inventory.TabIndex = 2;
            txt_inventory.Text = "{hospID:\"F\", stkCode:\"PH5\"}";
            // 
            // FormHsptlApiTest
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1500, 900);
            Controls.Add(gb_ipdrestore);
            Controls.Add(gb_ipdseq);
            Controls.Add(gb_returnhand);
            Controls.Add(gb_returnqrcode);
            Controls.Add(gb_outstore);
            Controls.Add(groupBox1);
            Controls.Add(gb_getdrug);
            Controls.Add(tb_log);
            Controls.Add(gb_getemp);
            Font = new Font("Microsoft JhengHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
            Name = "FormHsptlApiTest";
            Text = "Form";
            Load += Form_Load;
            gb_getemp.ResumeLayout(false);
            gb_getemp.PerformLayout();
            gb_getdrug.ResumeLayout(false);
            gb_getdrug.PerformLayout();
            gb_outstore.ResumeLayout(false);
            gb_outstore.PerformLayout();
            gb_returnqrcode.ResumeLayout(false);
            gb_returnqrcode.PerformLayout();
            gb_returnhand.ResumeLayout(false);
            gb_returnhand.PerformLayout();
            gb_ipdseq.ResumeLayout(false);
            gb_ipdseq.PerformLayout();
            gb_ipdrestore.ResumeLayout(false);
            gb_ipdrestore.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_getemp;
        private TextBox tb_getemp;
        private GroupBox gb_getemp;
        private TextBox tb_log;
        private GroupBox gb_getdrug;
        private Button bt_getdrug;
        private TextBox tb_getdrug;
        private GroupBox gb_outstore;
        private Button bt_outstore;
        private TextBox tb_outstore;
        private GroupBox gb_returnqrcode;
        private Button bt_returnqrcode;
        private TextBox tb_returnqrcode;
        private GroupBox gb_returnhand;
        private Button bt_returnhand;
        private TextBox tb_returnhand4;
        private TextBox tb_returnhand3;
        private TextBox tb_returnhand2;
        private TextBox tb_returnhand1;
        private GroupBox gb_ipdseq;
        private Button bt_ipdseq;
        private TextBox tb_ipdseq2;
        private TextBox tb_ipdseq1;
        private GroupBox gb_ipdrestore;
        private Button bt_ipdrestore;
        private TextBox tb_ipdrestore4;
        private TextBox tb_ipdrestore3;
        private TextBox tb_ipdrestore2;
        private TextBox tb_ipdrestore1;
        private GroupBox groupBox1;
        private Button btn_inventory;
        private TextBox txt_inventory;
    }
}