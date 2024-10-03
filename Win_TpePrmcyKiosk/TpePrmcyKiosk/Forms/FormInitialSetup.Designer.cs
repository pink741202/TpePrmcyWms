
namespace TpePrmcyKiosk
{
    partial class FormInitialSetup
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
            txb_devaddr = new TextBox();
            lb_devaddr = new Label();
            btn_conection = new Button();
            ckb_port1 = new CheckBox();
            ckb_port2 = new CheckBox();
            ckb_port3 = new CheckBox();
            ckb_port4 = new CheckBox();
            ckb_port5 = new CheckBox();
            ckb_port6 = new CheckBox();
            ckb_port7 = new CheckBox();
            ckb_port8 = new CheckBox();
            btn_setInitial = new Button();
            btn_setweight = new Button();
            txb_cmdlog = new TextBox();
            lb_weightval1 = new Label();
            lb_weightval2 = new Label();
            lb_weightval3 = new Label();
            lb_weightval4 = new Label();
            lb_weightval5 = new Label();
            lb_weightval6 = new Label();
            lb_weightval7 = new Label();
            lb_weightval8 = new Label();
            btn_getweight = new Button();
            btn_setempty = new Button();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txb_devaddr
            // 
            txb_devaddr.Location = new Point(149, 84);
            txb_devaddr.Margin = new Padding(4);
            txb_devaddr.Name = "txb_devaddr";
            txb_devaddr.Size = new Size(40, 32);
            txb_devaddr.TabIndex = 0;
            txb_devaddr.Text = "01";
            // 
            // lb_devaddr
            // 
            lb_devaddr.AutoSize = true;
            lb_devaddr.Location = new Point(29, 91);
            lb_devaddr.Margin = new Padding(4, 0, 4, 0);
            lb_devaddr.Name = "lb_devaddr";
            lb_devaddr.Size = new Size(86, 24);
            lb_devaddr.TabIndex = 1;
            lb_devaddr.Text = "設備位址";
            // 
            // btn_conection
            // 
            btn_conection.Location = new Point(29, 23);
            btn_conection.Margin = new Padding(4, 3, 4, 3);
            btn_conection.Name = "btn_conection";
            btn_conection.Size = new Size(234, 42);
            btn_conection.TabIndex = 2;
            btn_conection.Text = "取得連線";
            btn_conection.UseVisualStyleBackColor = true;
            btn_conection.Click += btn_conection_Click;
            // 
            // ckb_port1
            // 
            ckb_port1.AutoSize = true;
            ckb_port1.Location = new Point(42, 144);
            ckb_port1.Margin = new Padding(4, 3, 4, 3);
            ckb_port1.Name = "ckb_port1";
            ckb_port1.Size = new Size(97, 28);
            ckb_port1.TabIndex = 3;
            ckb_port1.Text = "磅秤埠1";
            ckb_port1.UseVisualStyleBackColor = true;
            ckb_port1.CheckedChanged += ckb_port1_CheckedChanged;
            // 
            // ckb_port2
            // 
            ckb_port2.AutoSize = true;
            ckb_port2.Location = new Point(42, 183);
            ckb_port2.Margin = new Padding(4, 3, 4, 3);
            ckb_port2.Name = "ckb_port2";
            ckb_port2.Size = new Size(97, 28);
            ckb_port2.TabIndex = 4;
            ckb_port2.Text = "磅秤埠2";
            ckb_port2.UseVisualStyleBackColor = true;
            ckb_port2.CheckedChanged += ckb_port2_CheckedChanged;
            // 
            // ckb_port3
            // 
            ckb_port3.AutoSize = true;
            ckb_port3.Location = new Point(42, 222);
            ckb_port3.Margin = new Padding(4, 3, 4, 3);
            ckb_port3.Name = "ckb_port3";
            ckb_port3.Size = new Size(97, 28);
            ckb_port3.TabIndex = 5;
            ckb_port3.Text = "磅秤埠3";
            ckb_port3.UseVisualStyleBackColor = true;
            ckb_port3.CheckedChanged += ckb_port3_CheckedChanged;
            // 
            // ckb_port4
            // 
            ckb_port4.AutoSize = true;
            ckb_port4.Location = new Point(42, 261);
            ckb_port4.Margin = new Padding(4, 3, 4, 3);
            ckb_port4.Name = "ckb_port4";
            ckb_port4.Size = new Size(97, 28);
            ckb_port4.TabIndex = 6;
            ckb_port4.Text = "磅秤埠4";
            ckb_port4.UseVisualStyleBackColor = true;
            ckb_port4.CheckedChanged += ckb_port4_CheckedChanged;
            // 
            // ckb_port5
            // 
            ckb_port5.AutoSize = true;
            ckb_port5.Location = new Point(42, 300);
            ckb_port5.Margin = new Padding(4, 3, 4, 3);
            ckb_port5.Name = "ckb_port5";
            ckb_port5.Size = new Size(97, 28);
            ckb_port5.TabIndex = 7;
            ckb_port5.Text = "磅秤埠5";
            ckb_port5.UseVisualStyleBackColor = true;
            ckb_port5.CheckedChanged += ckb_port5_CheckedChanged;
            // 
            // ckb_port6
            // 
            ckb_port6.AutoSize = true;
            ckb_port6.Location = new Point(42, 339);
            ckb_port6.Margin = new Padding(4, 3, 4, 3);
            ckb_port6.Name = "ckb_port6";
            ckb_port6.Size = new Size(97, 28);
            ckb_port6.TabIndex = 8;
            ckb_port6.Text = "磅秤埠6";
            ckb_port6.UseVisualStyleBackColor = true;
            ckb_port6.CheckedChanged += ckb_port6_CheckedChanged;
            // 
            // ckb_port7
            // 
            ckb_port7.AutoSize = true;
            ckb_port7.Location = new Point(42, 378);
            ckb_port7.Margin = new Padding(4, 3, 4, 3);
            ckb_port7.Name = "ckb_port7";
            ckb_port7.Size = new Size(97, 28);
            ckb_port7.TabIndex = 9;
            ckb_port7.Text = "磅秤埠7";
            ckb_port7.UseVisualStyleBackColor = true;
            ckb_port7.CheckedChanged += ckb_port7_CheckedChanged;
            // 
            // ckb_port8
            // 
            ckb_port8.AutoSize = true;
            ckb_port8.Location = new Point(42, 417);
            ckb_port8.Margin = new Padding(4, 3, 4, 3);
            ckb_port8.Name = "ckb_port8";
            ckb_port8.Size = new Size(97, 28);
            ckb_port8.TabIndex = 10;
            ckb_port8.Text = "磅秤埠8";
            ckb_port8.UseVisualStyleBackColor = true;
            ckb_port8.CheckedChanged += ckb_port8_CheckedChanged;
            // 
            // btn_setInitial
            // 
            btn_setInitial.Location = new Point(19, 37);
            btn_setInitial.Margin = new Padding(4, 3, 4, 3);
            btn_setInitial.Name = "btn_setInitial";
            btn_setInitial.Size = new Size(103, 63);
            btn_setInitial.TabIndex = 11;
            btn_setInitial.Text = "所有設定(0g)";
            btn_setInitial.UseVisualStyleBackColor = true;
            btn_setInitial.Click += btn_setInitial_Click;
            // 
            // btn_setweight
            // 
            btn_setweight.Location = new Point(130, 37);
            btn_setweight.Margin = new Padding(4, 3, 4, 3);
            btn_setweight.Name = "btn_setweight";
            btn_setweight.Size = new Size(98, 63);
            btn_setweight.TabIndex = 12;
            btn_setweight.Text = "法碼設定(100g)";
            btn_setweight.UseVisualStyleBackColor = true;
            btn_setweight.Click += btn_setweight_Click;
            // 
            // txb_cmdlog
            // 
            txb_cmdlog.Location = new Point(376, 80);
            txb_cmdlog.Margin = new Padding(4, 3, 4, 3);
            txb_cmdlog.Multiline = true;
            txb_cmdlog.Name = "txb_cmdlog";
            txb_cmdlog.ScrollBars = ScrollBars.Vertical;
            txb_cmdlog.Size = new Size(776, 522);
            txb_cmdlog.TabIndex = 13;
            // 
            // lb_weightval1
            // 
            lb_weightval1.AutoSize = true;
            lb_weightval1.Location = new Point(226, 144);
            lb_weightval1.Margin = new Padding(4, 0, 4, 0);
            lb_weightval1.Name = "lb_weightval1";
            lb_weightval1.Size = new Size(36, 24);
            lb_weightval1.TabIndex = 14;
            lb_weightval1.Text = "0.0";
            // 
            // lb_weightval2
            // 
            lb_weightval2.AutoSize = true;
            lb_weightval2.Location = new Point(226, 184);
            lb_weightval2.Margin = new Padding(4, 0, 4, 0);
            lb_weightval2.Name = "lb_weightval2";
            lb_weightval2.Size = new Size(36, 24);
            lb_weightval2.TabIndex = 15;
            lb_weightval2.Text = "0.0";
            // 
            // lb_weightval3
            // 
            lb_weightval3.AutoSize = true;
            lb_weightval3.Location = new Point(226, 223);
            lb_weightval3.Margin = new Padding(4, 0, 4, 0);
            lb_weightval3.Name = "lb_weightval3";
            lb_weightval3.Size = new Size(36, 24);
            lb_weightval3.TabIndex = 16;
            lb_weightval3.Text = "0.0";
            // 
            // lb_weightval4
            // 
            lb_weightval4.AutoSize = true;
            lb_weightval4.Location = new Point(226, 265);
            lb_weightval4.Margin = new Padding(4, 0, 4, 0);
            lb_weightval4.Name = "lb_weightval4";
            lb_weightval4.Size = new Size(36, 24);
            lb_weightval4.TabIndex = 17;
            lb_weightval4.Text = "0.0";
            // 
            // lb_weightval5
            // 
            lb_weightval5.AutoSize = true;
            lb_weightval5.Location = new Point(226, 305);
            lb_weightval5.Margin = new Padding(4, 0, 4, 0);
            lb_weightval5.Name = "lb_weightval5";
            lb_weightval5.Size = new Size(36, 24);
            lb_weightval5.TabIndex = 18;
            lb_weightval5.Text = "0.0";
            // 
            // lb_weightval6
            // 
            lb_weightval6.AutoSize = true;
            lb_weightval6.Location = new Point(226, 343);
            lb_weightval6.Margin = new Padding(4, 0, 4, 0);
            lb_weightval6.Name = "lb_weightval6";
            lb_weightval6.Size = new Size(36, 24);
            lb_weightval6.TabIndex = 19;
            lb_weightval6.Text = "0.0";
            // 
            // lb_weightval7
            // 
            lb_weightval7.AutoSize = true;
            lb_weightval7.Location = new Point(226, 383);
            lb_weightval7.Margin = new Padding(4, 0, 4, 0);
            lb_weightval7.Name = "lb_weightval7";
            lb_weightval7.Size = new Size(36, 24);
            lb_weightval7.TabIndex = 20;
            lb_weightval7.Text = "0.0";
            // 
            // lb_weightval8
            // 
            lb_weightval8.AutoSize = true;
            lb_weightval8.Location = new Point(226, 422);
            lb_weightval8.Margin = new Padding(4, 0, 4, 0);
            lb_weightval8.Name = "lb_weightval8";
            lb_weightval8.Size = new Size(36, 24);
            lb_weightval8.TabIndex = 21;
            lb_weightval8.Text = "0.0";
            // 
            // btn_getweight
            // 
            btn_getweight.Location = new Point(216, 80);
            btn_getweight.Margin = new Padding(4, 3, 4, 3);
            btn_getweight.Name = "btn_getweight";
            btn_getweight.Size = new Size(126, 49);
            btn_getweight.TabIndex = 22;
            btn_getweight.Text = "取得重量";
            btn_getweight.UseVisualStyleBackColor = true;
            btn_getweight.Click += btn_getweight_Click;
            // 
            // btn_setempty
            // 
            btn_setempty.Location = new Point(42, 568);
            btn_setempty.Margin = new Padding(4, 3, 4, 3);
            btn_setempty.Name = "btn_setempty";
            btn_setempty.Size = new Size(228, 46);
            btn_setempty.TabIndex = 12;
            btn_setempty.Text = "清空磅秤";
            btn_setempty.UseVisualStyleBackColor = true;
            btn_setempty.Click += btn_setempty_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btn_setInitial);
            groupBox1.Controls.Add(btn_setweight);
            groupBox1.Location = new Point(42, 451);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(300, 111);
            groupBox1.TabIndex = 23;
            groupBox1.TabStop = false;
            groupBox1.Text = "批次初始設定";
            // 
            // FormInitialSetup
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1200, 654);
            Controls.Add(groupBox1);
            Controls.Add(btn_getweight);
            Controls.Add(lb_weightval8);
            Controls.Add(lb_weightval7);
            Controls.Add(lb_weightval6);
            Controls.Add(lb_weightval5);
            Controls.Add(lb_weightval4);
            Controls.Add(lb_weightval3);
            Controls.Add(lb_weightval2);
            Controls.Add(lb_weightval1);
            Controls.Add(txb_cmdlog);
            Controls.Add(btn_setempty);
            Controls.Add(ckb_port8);
            Controls.Add(ckb_port7);
            Controls.Add(ckb_port6);
            Controls.Add(ckb_port5);
            Controls.Add(ckb_port4);
            Controls.Add(ckb_port3);
            Controls.Add(ckb_port2);
            Controls.Add(ckb_port1);
            Controls.Add(btn_conection);
            Controls.Add(lb_devaddr);
            Controls.Add(txb_devaddr);
            Font = new Font("微軟正黑體", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
            Name = "FormInitialSetup";
            Text = "FormInitialSetup";
            FormClosing += FormInitialSetup_FormClosing;
            Load += FormInitialSetup_Load;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txb_devaddr;
        private Label lb_devaddr;
        private Button btn_conection;
        private CheckBox ckb_port1;
        private CheckBox ckb_port2;
        private CheckBox ckb_port3;
        private CheckBox ckb_port4;
        private CheckBox ckb_port5;
        private CheckBox ckb_port6;
        private CheckBox ckb_port7;
        private CheckBox ckb_port8;
        private Button btn_setInitial;
        private Button btn_setweight;
        private TextBox txb_cmdlog;
        private Label lb_weightval1;
        private Label lb_weightval2;
        private Label lb_weightval3;
        private Label lb_weightval4;
        private Label lb_weightval5;
        private Label lb_weightval6;
        private Label lb_weightval7;
        private Label lb_weightval8;
        private Button btn_getweight;
        private Button btn_setempty;
        private GroupBox groupBox1;
    }
}