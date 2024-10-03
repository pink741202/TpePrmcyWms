
namespace TpePrmcyKiosk
{
    partial class FormDeviceDetect
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
            pnl_Drawers = new Panel();
            lb_CbntName = new Label();
            cb_action = new ComboBox();
            lb_action = new Label();
            txt_ResultList = new TextBox();
            lb_ResultList = new Label();
            lb_Drawers = new Label();
            btn_Start = new Button();
            SuspendLayout();
            // 
            // pnl_Drawers
            // 
            pnl_Drawers.BackColor = SystemColors.InactiveCaption;
            pnl_Drawers.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point);
            pnl_Drawers.Location = new Point(34, 137);
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
            // cb_action
            // 
            cb_action.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_action.Font = new Font("微軟正黑體", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            cb_action.FormattingEnabled = true;
            cb_action.Items.AddRange(new object[] { "已壞掉設備", "單抽屜", "全櫃設備" });
            cb_action.Location = new Point(1096, 25);
            cb_action.Name = "cb_action";
            cb_action.Size = new Size(190, 42);
            cb_action.TabIndex = 5;
            cb_action.SelectedIndexChanged += cb_action_SelectedIndexChanged;
            // 
            // lb_action
            // 
            lb_action.AutoSize = true;
            lb_action.Font = new Font("微軟正黑體", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb_action.Location = new Point(764, 33);
            lb_action.Name = "lb_action";
            lb_action.Size = new Size(312, 34);
            lb_action.TabIndex = 6;
            lb_action.Text = "選擇偵測並確認修復範圍";
            // 
            // txt_ResultList
            // 
            txt_ResultList.Font = new Font("微軟正黑體", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            txt_ResultList.Location = new Point(764, 137);
            txt_ResultList.Multiline = true;
            txt_ResultList.Name = "txt_ResultList";
            txt_ResultList.ReadOnly = true;
            txt_ResultList.ScrollBars = ScrollBars.Vertical;
            txt_ResultList.Size = new Size(948, 751);
            txt_ResultList.TabIndex = 7;
            // 
            // lb_ResultList
            // 
            lb_ResultList.AutoSize = true;
            lb_ResultList.Font = new Font("微軟正黑體", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb_ResultList.Location = new Point(764, 100);
            lb_ResultList.Name = "lb_ResultList";
            lb_ResultList.Size = new Size(123, 34);
            lb_ResultList.TabIndex = 6;
            lb_ResultList.Text = "偵測清單";
            // 
            // lb_Drawers
            // 
            lb_Drawers.AutoSize = true;
            lb_Drawers.Font = new Font("微軟正黑體", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lb_Drawers.Location = new Point(34, 100);
            lb_Drawers.Name = "lb_Drawers";
            lb_Drawers.Size = new Size(123, 34);
            lb_Drawers.TabIndex = 6;
            lb_Drawers.Text = "本櫃抽屜";
            // 
            // btn_Start
            // 
            btn_Start.Location = new Point(1415, 25);
            btn_Start.Name = "btn_Start";
            btn_Start.Size = new Size(129, 42);
            btn_Start.TabIndex = 8;
            btn_Start.Text = "開始";
            btn_Start.UseVisualStyleBackColor = true;
            btn_Start.Click += btn_Start_Click;
            // 
            // FormDeviceDetect
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(1800, 970);
            Controls.Add(btn_Start);
            Controls.Add(txt_ResultList);
            Controls.Add(lb_Drawers);
            Controls.Add(lb_ResultList);
            Controls.Add(lb_action);
            Controls.Add(cb_action);
            Controls.Add(lb_CbntName);
            Controls.Add(pnl_Drawers);
            Font = new Font("微軟正黑體", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(5);
            Name = "FormDeviceDetect";
            Text = "FormComuTaskTest";
            FormClosing += FormComuTaskTest_FormClosing;
            Load += FormComuTaskTest_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnl_Drawers;
        private Label lb_CbntName;
        private ComboBox cb_action;
        private Label lb_action;
        private TextBox txt_ResultList;
        private Label lb_ResultList;
        private Label lb_Drawers;
        private Button btn_Start;
    }
}