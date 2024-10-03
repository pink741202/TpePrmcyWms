using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TpePrmcyKiosk
{
    public partial class FormPopupMessage : Form
    {
        string ShowMsg = "";
        public FormPopupMessage(string msg)
        {
            InitializeComponent();
            ShowMsg = msg;
        }

        private void FormMessage_Load(object sender, EventArgs e)
        {
            lb_show.Text = ShowMsg;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
