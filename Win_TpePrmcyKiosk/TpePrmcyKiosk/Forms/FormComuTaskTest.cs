using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using TpePrmcyWms.Models.DOM;
using TpePrmcyKiosk.Models.DOM;
using TpePrmcyKiosk.Models.Service;
using TpePrmcyKiosk.Models.Unit;

namespace TpePrmcyKiosk
{
    public partial class FormComuTaskTest : Form
    {
        private MainForm _parent;
        int AtCbntFid = Convert.ToInt32(ConfigurationManager.AppSettings["AtCbntFid"]);

        SsrQuService quServ = new SsrQuService();

        public FormComuTaskTest(MainForm parent)
        {
            InitializeComponent();
            _parent = parent;
            quServ.ssrQuUpdateEvent += QuUpdateHandler;
        }

        private void FormComuTaskTest_Load(object sender, EventArgs e)
        {
            try
            {
                using (DBcPharmacy db = new DBcPharmacy())
                {
                    db.SensorComuQuee.RemoveRange(db.SensorComuQuee.ToList());
                    db.SaveChanges();

                    lb_CbntName.Text = db.Cabinet.Find(AtCbntFid).CbntName;

                    List<Drawers> drawers = db.Drawers.Where(x => x.CbntFid == AtCbntFid).OrderBy(x => x.No).ToList();
                    int startX = 5, startY = 5;
                    int width = 60, heigh = 50;
                    int gap = 5;
                    int colcount = 10;

                    this.pnl_Drawers.Width = (width + gap) * 10 + (startX * 2);
                    this.pnl_Drawers.Height = (heigh + gap) * ((drawers.Count / 10) + 1) + (startY * 2);
                    for (int i = 0; i < drawers.Count; i++)
                    {
                        int locatX = (width + gap) * (i % 10) + startX;
                        int locatY = (heigh + gap) * (i / 10) + startY;

                        Button newButton = new Button();
                        newButton.Name = $"Drawer_{drawers[i].FID}";
                        newButton.Text = $"# {drawers[i].No}";
                        newButton.Location = new Point(locatX, locatY);
                        newButton.Size = new Size(width, heigh);
                        newButton.Click += new EventHandler(btn_Drawer_Click);

                        this.pnl_Drawers.Controls.Add(newButton);

                    }
                }
                quServ.RunLoop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("連接資料庫失敗，無法操作！");

            }
        }

        private void btn_Drawer_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.Enabled = false;
            int getFid = Convert.ToInt32(btn.Name.Replace("Drawer_", string.Empty));
            int taskcnt = this.pnl_WaitLine.Controls.Count / 2;
            if (taskcnt >= 4) { MessageBox.Show("序列滿囉 =.=!!"); return; }
            #region add db record
            try
            {
                SensorComuQuee qu = new SensorComuQuee()
                {
                    GID = new Guid(),
                    CbntFid = AtCbntFid,
                    LEDColor = "",
                    DrawFid = getFid,
                    oprState = "0", //預設等led狀態
                    DrugCode = "Kiosk",
                    OperType = "Kio",
                };

                using (DBcPharmacy db = new DBcPharmacy())
                {
                    db.Add(qu);
                    db.SaveChanges();
                    db.Dispose();
                }
            }
            catch (Exception ex) { }
            #endregion

            #region add qu ComboBox in view
            ComboBox cbbox = new ComboBox();
            cbbox.Name = $"cb_task_{getFid}";
            //cbbox.BackColor = SystemColors.MenuHighlight;
            cbbox.FormattingEnabled = true;
            cbbox.Items.AddRange(new object[] { "等待中", "操作中", "操作完成" });
            cbbox.Location = new Point(30, (taskcnt * 45) + 15);
            cbbox.Size = new Size(156, 30);
            cbbox.SelectedIndex = 0;
            cbbox.Enabled = false;
            cbbox.SelectedIndexChanged += new EventHandler(cbb_Task_Change);
            this.pnl_WaitLine.Controls.Add(cbbox);

            Label nlbl = new Label();
            nlbl.Name = $"lb_task_{getFid}";
            nlbl.Location = new Point(192, (taskcnt * 45) + 15);
            nlbl.Size = new Size(140, 30);
            nlbl.Text = "未開門";
            this.pnl_WaitLine.Controls.Add(nlbl);
            #endregion
        }

        private void cbb_Task_Change(object sender, EventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            int getFid = Convert.ToInt32(cbb.Name.Replace("cb_task_", string.Empty));
            if (cbb.SelectedIndex == 2)
            {
                try
                {
                    using (DBcPharmacy db = new DBcPharmacy())
                    {
                        SensorComuQuee obj = db.SensorComuQuee.Where(x => x.CbntFid == AtCbntFid && x.DrawFid == getFid).First();
                        obj.oprState = "2";
                        db.Update(obj);
                        db.SaveChanges();
                    }
                }
                catch { }
            }
        }

        private void FormComuTaskTest_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        #region handler
        private void QuUpdateHandler(object sender, QuTaskArgs e)
        {
            SensorComuQuee obj = e.QuObj;

            //模擬前台改狀態
            if (obj.oprState == "0" && obj.LEDColor != "")
            {
                List<Control> drawers = pnl_Drawers.Controls.Find($"Drawer_{obj.DrawFid}", false).ToList();
                if (drawers.Count > 0)
                {
                    Button drawer = (Button)drawers[0];
                    drawer.BackColor = qwFunc.toColor(obj.LEDColor);
                }
                List<Control> tasks = pnl_WaitLine.Controls.Find($"cb_task_{obj.DrawFid}", false).ToList();
                if (tasks.Count > 0)
                {
                    ComboBox task = (ComboBox)tasks[0];
                    task.BackColor = qwFunc.toColor(obj.LEDColor);
                    task.Enabled = true;
                    task.SelectedIndex = 1;
                    //模擬前台自動改狀態
                    obj.oprState = "1";
                    try
                    {
                        using (DBcPharmacy db = new DBcPharmacy())
                        {
                            db.Update(obj);
                            db.SaveChanges();
                            db.Dispose();
                        };
                    }
                    catch { }
                }
            }

            if (obj.ssrState == "1" || obj.ssrState == "2")
            {
                List<Control> tasks = pnl_WaitLine.Controls.Find($"lb_task_{obj.DrawFid}", false).ToList();
                if (tasks.Count > 0)
                {
                    Label task = (Label)tasks[0];
                    switch (obj.ssrState)
                    {
                        case "1": task.Text = "開門中"; break;
                        case "2": task.Text = "已關門"; break;
                    }
                }
            }

            if (obj.ssrState == "D")
            {//InactiveCaption
                List<Control> drawers = pnl_Drawers.Controls.Find($"Drawer_{obj.DrawFid}", false).ToList();
                if (drawers.Count > 0)
                {
                    Button btn = (Button)drawers[0];
                    btn.BackColor = SystemColors.InactiveCaption;
                    btn.Enabled = true;
                }

                List<Control> tasks = pnl_WaitLine.Controls.Find($"cb_task_{obj.DrawFid}", false).ToList();
                if (tasks.Count > 0)
                {
                    ComboBox cbb = (ComboBox)tasks[0];
                    pnl_WaitLine.Controls.Remove(cbb);
                }
                List<Control> taskslb = pnl_WaitLine.Controls.Find($"lb_task_{obj.DrawFid}", false).ToList();
                if (tasks.Count > 0)
                {
                    Label lb = (Label)taskslb[0];
                    pnl_WaitLine.Controls.Remove(lb);
                }

                List<ComboBox> cbbs = pnl_WaitLine.Controls.OfType<ComboBox>().ToList();
                List<Label> lbs = pnl_WaitLine.Controls.OfType<Label>().ToList();
                for (int i = 0; i < cbbs.Count; i++)
                {
                    ComboBox cb = cbbs[i];
                    Label lb = lbs[i];
                    cb.Location = new Point(30, (i * 45) + 15);
                    lb.Location = new Point(192, (i * 45) + 15);
                }

            }


        }
        #endregion
    }
}
