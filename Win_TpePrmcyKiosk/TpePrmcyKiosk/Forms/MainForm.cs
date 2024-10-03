using System.Configuration;
using TpePrmcyKiosk.Models.Unit;

namespace TpePrmcyKiosk
{
    public partial class MainForm : Form
    {
        public bool isTesting = ConfigurationManager.AppSettings["Testing"] == "1";
        public MainForm()
        {
            InitializeComponent();
            this.ControlBox = false;
            if (!isTesting)
            {
                this.WindowState = FormWindowState.Maximized; //全螢幕
                this.TopMost = true;
                menu_Setup.Visible = false;
                menu_Test.Visible = false;
            }
        }


        #region 切換頁面                
        public void LoadForm(object Form)
        {
            if (this.mainpanel.Controls.Count > 0)
                this.mainpanel.Controls.RemoveAt(0);
            Form f = (Form)Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            f.Width = mainpanel.Width;
            f.Height = mainpanel.Height;
            f.Top = 32;
            this.mainpanel.Controls.Add(f);
            this.mainpanel.Tag = f;
            f.Show();
        }
        private void menu_login_Click(object sender, EventArgs e)
        {
            LoadForm(new FormLogin());
        }
        private void menu_browser_Click(object sender, EventArgs e)
        {
            LoadForm(new FormBrowser(this));
        }
        private void Menu_CloseSys_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void menu_InitialScale_Click(object sender, EventArgs e)
        {
            LoadForm(new FormInitialSetup());
        }

        private void menu_ComuSensors_Click(object sender, EventArgs e)
        {
            LoadForm(new FormComuTaskTest(this));
        }
        private void menu_HsptlApi_Click(object sender, EventArgs e)
        {
            LoadForm(new FormHsptlApiTest(this));
        }
        private void LEDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadForm(new FormLEDStripTest(this));
        }

        private void menu_DeviceDetect_Click(object sender, EventArgs e)
        {
            LoadForm(new FormDeviceDetect(this));
        }

        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadForm(new FormHsptlApiTest(this));
        }

        private void btn_popup_Click(object sender, EventArgs e)
        {
            //test only
            //FormLogin f2 = new FormLogin();
            ////f2.Show();
            //f2.getvalue = "from form1";
            //f2.ShowDialog();

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            System.Environment.Exit(System.Environment.ExitCode);
            //StaticFunc.Alert("請回［首頁］ ＞ 點選［關閉系統］！");
        }


    }
}