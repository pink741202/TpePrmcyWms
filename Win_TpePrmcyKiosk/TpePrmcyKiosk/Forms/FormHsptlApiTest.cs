using TpePrmcyWms.Models.DOM;
using ShareLibrary.Models.HsptlApiUnit;
using ShareLibrary.Models.Service;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Xml;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TpePrmcyKiosk
{
    public partial class FormHsptlApiTest : Form
    {
        private MainForm _parent;
        HsptlApiService _apiserv = new HsptlApiService();
        public FormHsptlApiTest(MainForm parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            _parent.Top = 20;
            _parent.Left = 20;
            _parent.Width = 1500;
            _parent.Height = 950;
            this.Width = 1500;
            this.Height = 950;


        }

        private async void btn_getemp_Click(object sender, EventArgs e)
        {
            btn_getemp.Enabled = false;
            tb_log.AppendText($"btn_getemp_Click()" + Environment.NewLine);
            if (tb_getemp.Text == "") { btn_getemp.Enabled = true; return; }
            string emp_no = tb_getemp.Text;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync("http://hq-sso2-nvm/IWS/AJAX/getEmpInfo?emp_no=" + emp_no);
                    tb_log.AppendText(response + Environment.NewLine);
                    employee? re = await _apiserv.getEmpInfo(emp_no);
                    if (re == null) { tb_log.AppendText("失敗" + Environment.NewLine); }
                    else { tb_log.AppendText("成功" + Environment.NewLine); }
                }
            }
            catch (Exception ex) { tb_log.AppendText($"錯誤:{ex.Message}" + Environment.NewLine); }
            btn_getemp.Enabled = true;
        }

        private async void bt_getdrug_Click(object sender, EventArgs e)
        {
            bt_getdrug.Enabled = false;
            tb_log.AppendText($"bt_getdrug_Click()" + Environment.NewLine);
            if (tb_getdrug.Text == "") { bt_getdrug.Enabled = true; return; }
            string odr_code = tb_getdrug.Text;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync("http://hq-sso2-nvm/ESMC/Home/Detail?odr_code=" + odr_code);
                    tb_log.AppendText(response + Environment.NewLine);
                    DrugInfo? re = await _apiserv.getDrugInfo(odr_code);
                    if (re == null) { tb_log.AppendText("失敗" + Environment.NewLine); }
                    else { tb_log.AppendText("成功" + Environment.NewLine); }
                }
            }
            catch (Exception ex) { tb_log.AppendText($"錯誤:{ex.Message}" + Environment.NewLine); }
            bt_getdrug.Enabled = true;
        }

        private async void bt_outstore_Click(object sender, EventArgs e)
        {
            bt_outstore.Enabled = false;
            tb_log.AppendText($"bt_outstore_Click()" + Environment.NewLine);
            if (tb_outstore.Text == "") { bt_outstore.Enabled = true; return; }
            string[] strings = tb_outstore.Text.Split(";");
            if (strings.Length != 17) { bt_outstore.Enabled = true; return; }
            try
            {
                Qry_OutStorage qry = new Qry_OutStorage(strings);

                string url = $"http://hq-medwebsrv-vm:8086/PHA/GetOutStorage?HospId={qry.HospId}" +
                        $"&PatNo={qry.PatNo}&PatSeq={qry.PatSeq}&OdrSeq={qry.OdrSeq}" +
                        $"&PhaDate={qry.PhaDate}&PhaNum={qry.PhaNum}&OdrCode={qry.OdrCode}" +
                        $"&TotalQty={qry.TotalQty}&Dose={qry.Dose}&Freqcode={qry.Freqcode}&Days={qry.Days}";

                string mApiurl = "http://hq-medwebsrv-vm:8086/PHA/GetOutStorage";
                string mUrl = string.Format(mApiurl + "?HospId={0}&PatNo={1}&PatSeq={2}&OdrSeq={3}&PhaDate={4}&PhaNum={5}&OdrCode={6}&TotalQty={7}",
                    qry.HospId, qry.PatNo, qry.PatSeq, qry.OdrSeq, qry.PhaDate, qry.PhaNum, qry.OdrCode, qry.TotalQty);



                tb_log.AppendText($"old:{_apiserv.ClientSend(mUrl)}" + Environment.NewLine);
                tb_log.AppendText($"new:{_apiserv.ClientSend(url)}" + Environment.NewLine);

            }
            catch (Exception ex) { tb_log.AppendText($"錯誤:{ex}" + Environment.NewLine); }
            bt_outstore.Enabled = true;
        }

        private async void bt_returnqrcode_Click(object sender, EventArgs e)
        {
            bt_returnqrcode.Enabled = false;
            tb_log.AppendText($"bt_returnqrcode_Click()" + Environment.NewLine);
            if (tb_returnqrcode.Text == "") { bt_returnqrcode.Enabled = true; return; }
            string[] strings = tb_returnqrcode.Text.Split(";");
            if (strings.Length != 17) { bt_returnqrcode.Enabled = true; return; }
            try
            {
                Qry_ReturnByQRCode qry = new Qry_ReturnByQRCode(strings);
                string url = $"http://hq-medwebsrv-vm:8086/PHA/GetOPDReturnStorageByQRCode?" +
                        $"HospId={qry.HospId}&PatNo={qry.PatNo}&PatSeq={qry.PatSeq}";
                tb_log.AppendText($"{_apiserv.ClientSend(url)}" + Environment.NewLine);
            }
            catch (Exception ex) { tb_log.AppendText($"錯誤:{ex}" + Environment.NewLine); }
            bt_returnqrcode.Enabled = true;
        }

        private async void bt_returnhand_Click(object sender, EventArgs e)
        {
            bt_returnhand.Enabled = false;
            tb_log.AppendText($"bt_returnhand_Click()" + Environment.NewLine);
            if (tb_returnhand1.Text == "" || tb_returnhand2.Text == ""
                ) { bt_returnhand.Enabled = true; return; }
            try
            {
                Qry_ReturnByHand qry = new Qry_ReturnByHand();
                qry.HospId = tb_returnhand1.Text;
                qry.PatNo = tb_returnhand2.Text;
                qry.PhaDate = tb_returnhand3.Text;
                qry.PhaNum = tb_returnhand4.Text;

                using (HttpClient client = new HttpClient())
                {
                    string url = $"http://hq-medwebsrv-vm:8086/PHA/GetOPDReturnStorageByHand?" +
                        $"HospId={qry.HospId}&PatNo={qry.PatNo}&PhaDate={qry.PhaDate}&PhaNum={qry.PhaNum}";
                    tb_log.AppendText($"{_apiserv.ClientSend(url)}" + Environment.NewLine);
                }
            }
            catch (Exception ex) { tb_log.AppendText($"錯誤:{ex}" + Environment.NewLine); }
            bt_returnhand.Enabled = true;
        }

        private async void bt_ipdseq_Click(object sender, EventArgs e)
        {
            bt_ipdseq.Enabled = false;
            tb_log.AppendText($"bt_ipdseq_Click()" + Environment.NewLine);
            if (tb_ipdseq1.Text == "" || tb_ipdseq2.Text == "") { bt_ipdseq.Enabled = true; return; }
            try
            {
                Qry_IPDPatSeq qry = new Qry_IPDPatSeq();
                qry.HospId = tb_ipdseq1.Text;
                qry.PatNo = tb_ipdseq2.Text;

                //string url = $"http://hq-medwebsrv-vm:8086/PHA/GetIPDPatSeq?HospId={qry.HospId}&PatNo={qry.PatNo}";
                var apiurl2 = "http://hq-medwebsrv-vm:8086/PHA/GetIPDPatSeq";
                var url2 = string.Format(apiurl2 + "?HospId={0}&PatNo={1}", qry.HospId, qry.PatNo);
                tb_log.AppendText($"{_apiserv.ClientSend(url2)}" + Environment.NewLine);
            }
            catch (Exception ex) { tb_log.AppendText($"錯誤:{ex}" + Environment.NewLine); }
            bt_ipdseq.Enabled = true;
        }

        private async void bt_ipdrestore_Click(object sender, EventArgs e)
        {
            bt_ipdrestore.Enabled = false;
            tb_log.AppendText($"bt_ipdrestore_Click()" + Environment.NewLine);
            //if (tb_ipdrestore1.Text == "" || tb_ipdrestore2.Text == "" || tb_ipdrestore3.Text == "" || tb_ipdrestore4.Text == "") { bt_ipdrestore.Enabled = true; return; }
            try
            {
                Qry_IPDReturn qry = new Qry_IPDReturn();
                qry.HospId = tb_ipdrestore1.Text;
                qry.PatNo = tb_ipdrestore2.Text;
                qry.PatSeq = tb_ipdrestore3.Text;
                qry.ReturnSheet = tb_ipdrestore4.Text;

                var apiurl2 = "http://hq-medwebsrv-vm:8086/PHA/GetIPDReturnStorage";
                var url2 = string.Format(apiurl2 + "?HospId={0}&PatNo={1}&PatSeq={2}&ReturnSheet={3}",
                    qry.HospId, qry.PatNo, qry.PatSeq, qry.ReturnSheet);

            }
            catch (Exception ex) { tb_log.AppendText($"錯誤:{ex}" + Environment.NewLine); }
            bt_ipdrestore.Enabled = true;
        }

        private async void bt_getInventory_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://10.200.20.115:9101/InventoryService.asmx/GetInventory");
            var content = new StringContent("{\"hospId\":\"F\",\"stkCode\":\"PH5\"}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            tb_log.AppendText(await response.Content.ReadAsStringAsync());

        }

    }
}
