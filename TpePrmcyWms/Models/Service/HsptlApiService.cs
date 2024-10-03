using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;
using System.Xml.Linq;
using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.HsptlApiUnit;
using TpePrmcyWms.Models.Unit;
using TpePrmcyWms.Models.Unit.Back;

namespace TpePrmcyWms.Models.Service
{
    public class HsptlApiService
    {
        DBcPharmacy _db = new DBcPharmacy();
        private LoginInfo _loginfo;
        public HsptlApiService(LoginInfo loginfo) {
            _loginfo = loginfo;
        }

        private string ClientSend(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.Encoding = Encoding.UTF8;
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    webClient.Headers.Add("authorization", "token {apitoken}");
                    var result = webClient.UploadString(url, "");
                    return result;
                }
                catch (Exception ex)
                {
                    SysBaseServ.Log(_loginfo, ex, $"API Client [{url}] Sended Error：{ex}");
                    return $"ERR";
                }
            }
        }

        private void ApiCientFailedLog(string urlstring)
        {
            ApiLog add = new ApiLog
            {
                UrlString = urlstring,
                LogTime = DateTime.Now,
                Success = false,
            };
            _db.ApiLog.Add(add);
            _db.SaveChanges();
        }

        public async Task<ResponObj<employee?>> getEmpInfo(string emp_no)
        {
            ResponObj<employee?> result = new ResponObj<employee?>();
            if (string.IsNullOrEmpty(emp_no)) { return new ResponObj<employee?>("Err", null, "條件錯誤"); }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string mUrl = "http://hq-sso2-nvm/IWS/AJAX/getEmpInfo?emp_no=" + emp_no;
                    string response = await client.GetStringAsync(mUrl);
                    if(response != "ERR")
                    {
                        ApiEmployeeRoot? Root = JsonConvert.DeserializeObject<ApiEmployeeRoot>(response);
                        if (Root == null) { return new ResponObj<employee?>("Err", null, "查詢結果:null"); }
                        if (Root.status.ToLower() != "success" || Root.rows.Count == 0 || Root.rows.First().emp_no != emp_no) {
                            return new ResponObj<employee?>("Err", null, "查詢結果:失敗或無資料");
                        }
                        var row = Root.rows.First();
                        result.returnData = new employee
                        {
                            //empacc = row.emp_no,
                            emp_no = row.emp_no,
                            name = row.emp_name,
                            emp_dep_code = row.emp_dep_code,
                            emp_pos_code = row.emp_pos_code,
                            emp_pos_name = row.emp_pos_name,
                            emp_location = row.emp_location,
                            emp_cost_center = row.emp_cost_center,
                            emp_birth = row.emp_birth,
                            CardNo = Int32.Parse(row.CardNo, System.Globalization.NumberStyles.HexNumber).ToString(),
                            enname = row.emp_ename,
                            mobile_tel = row.mobile_tel,
                            mobile_code = row.mobile_code,
                            email = $"{row.emp_no}@tpech.gov.tw",
                        };
                        return result;
                    }
                    else { return new ResponObj<employee?>("Err", null, "查詢結果:Err"); }
                }
            }
            catch (Exception ex) { 
                SysBaseServ.Log(_loginfo, ex, $"API getEmpInfo({emp_no})");
                return new ResponObj<employee?>("Err", null, ex.Message);
            }
        }

        public async Task<ResponObj<DrugInfo?>> getDrugInfo(string odr_code)
        {
            ResponObj<DrugInfo?> result = new ResponObj<DrugInfo?>();
            if (string.IsNullOrEmpty(odr_code)) { return new ResponObj<DrugInfo?>("Err", null, "條件錯誤"); }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync("http://hq-sso2-nvm/ESMC/Home/Detail?odr_code=" + odr_code);
                    if (response != "ERR")
                    {
                        ApiMedicineRoot? Root = JsonConvert.DeserializeObject<ApiMedicineRoot>(response);
                        if (Root == null) { return new ResponObj<DrugInfo?>("Err", null, "查詢結果:null"); }
                        if (Root.status.ToLower() != "success" || Root.rows.Count == 0 || Root.rows.First().odr_code != odr_code)
                        {
                            return new ResponObj<DrugInfo?>("Err", null, "查詢結果:失敗或無資料");
                        }
                        var row = Root.rows.First();

                        result.returnData = new DrugInfo
                        {
                            DrugCode = row.odr_code,
                            DrugName = row.textinfo_01,
                            Info_01 = row.textinfo_01,
                            Info_02 = row.textinfo_02,
                            Info_03 = row.textinfo_03,
                            Info_04 = row.textinfo_04,
                            Info_05 = row.textinfo_05,
                            Info_06 = row.textinfo_06,
                            Info_07 = row.textinfo_07,
                            Info_08 = row.textinfo_08,
                            Info_09 = row.textinfo_09,
                            Info_10 = row.textinfo_10,
                            Info_11 = row.textinfo_11,
                            Info_12 = row.textinfo_12,
                            Info_13 = row.textinfo_13,
                            Info_14 = row.textinfo_14,
                            Info_15 = row.textinfo_15,
                            Info_16 = row.textinfo_16,
                            Info_17 = row.textinfo_17,
                            Info_18 = row.textinfo_18,
                            Info_19 = row.textinfo_19,
                            Info_20 = row.textinfo_20,
                            Info_21 = row.textinfo_21,
                            Info_22 = row.textinfo_22,
                            Info_23 = row.textinfo_23,
                            Info_24 = row.textinfo_24,
                            Info_25 = row.textinfo_25,
                            Info_26 = row.textinfo_26,
                            Info_27 = row.textinfo_27,
                            Info_28 = row.textinfo_28,
                            Info_29 = row.textinfo_29,
                            Info_30 = row.textinfo_30,
                        };
                        return result;
                    }
                    else
                    {
                        return new ResponObj<DrugInfo?>("Err", null, "查詢結果:ERR");
                    }
                }
            }
            catch (Exception ex) { 
                SysBaseServ.Log(_loginfo, ex, $"API getDrugInfo({odr_code})"); 
                return new ResponObj<DrugInfo?>("Err", null, ex.Message); 
            }
        }

        public bool? getOutStorage(ApiQueryObj qry)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string mApiurl = "http://hq-medwebsrv-vm:8086/PHA/GetOutStorage";
                    string mUrl = string.Format(mApiurl + "?HospId={0}&PatNo={1}&PatSeq={2}&OdrSeq={3}&PhaDate={4}&PhaNum={5}&OdrCode={6}&TotalQty={7}",
                        qry.HospId, qry.PatNo, qry.PatSeq, qry.OdrSeq, qry.PhaDate, qry.PhaNum, qry.OdrCode, qry.TotalQty);
                    string response = ClientSend(mUrl);
                    if (response != "ERR")
                    {
                        Resp_OutStorage resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Resp_OutStorage>(response);
                        if (resp.Succ) { return resp.Data; } else { return null; }
                    }
                    else { ApiCientFailedLog(mUrl); return null; }
                }
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex, $"Api getOutStorage({qry.OdrCode}...) 失敗:{ex}"); return null; }
            
        }

        public async Task<List<Data_ReturnByQRCode>> getOPDReturnStorageByQRCode(ApiQueryObj qry)
        {
            List<Data_ReturnByQRCode>? result;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"http://hq-medwebsrv-vm:8086/PHA/GetOPDReturnStorageByQRCode?" +
                        $"HospId={qry.HospId}&PatNo={qry.PatNo}&PatSeq={qry.PatSeq}";
                    string response = ClientSend(url);
                    if (response != "ERR")
                    {
                        Resp_ReturnByQRCode resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Resp_ReturnByQRCode>(response);
                        if (resp.Succ) { return resp.Data; } else { return null; }
                    }
                    else { ApiCientFailedLog(url); return null; }
                }
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex, $"Api getOPDReturnStorageByQRCode({qry.PatNo}...) 失敗:{ex}"); return null; }
        }

        public async Task<List<Data_ReturnByHand>> getOPDReturnStorageByHand(ApiQueryObj qry)
        {
            List<Data_ReturnByHand>? result;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"http://hq-medwebsrv-vm:8086/PHA/GetOPDReturnStorageByHand?" +
                        $"HospId={qry.HospId}&PatNo={qry.PatNo}&PhaDate={qry.PhaDate}&PhaNum={qry.PhaNum}";
                    string response = ClientSend(url);
                    if (response != "ERR")
                    {
                        Resp_ReturnByHand resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Resp_ReturnByHand>(response);
                        if (resp.Succ) { return resp.Data; } else { return null; }
                    }
                    else { ApiCientFailedLog(url); return null; }
                }
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex, $"Api getOPDReturnStorageByHand({qry.PatNo}...) 失敗:{ex}"); return null; }
        }

        public async Task<List<Data_IPDPatSeq>> getIPDPatSeq(ApiQueryObj qry)
        {
            List<Data_IPDPatSeq>? result;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string mApiurl = "http://hq-medwebsrv-vm:8086/PHA/GetIPDPatSeq";
                    string mUrl = string.Format(mApiurl + "?HospId={0}&PatNo={1}", qry.HospId, qry.PatNo);
                    string response = ClientSend(mUrl);
                    if (response != "ERR")
                    {
                        Resp_IPDPatSeq resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Resp_IPDPatSeq>(response);
                        if (resp.Succ) { return resp.Data; } else { return null; }
                    }
                    else { ApiCientFailedLog(mUrl); return null; }
                }
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex, $"Api getIPDPatSeq({qry.PatNo}...) 失敗:{ex}"); return null; }
        }

        public async Task<List<Data_IPDReturn>> getIPDReturnStorage(ApiQueryObj qry)
        {
            List<Data_IPDReturn>? result;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string mApiurl = "http://hq-medwebsrv-vm:8086/PHA/GetIPDReturnStorage";
                    string mUrl = string.Format(mApiurl + "?HospId={0}&PatNo={1}&PatSeq={2}&ReturnSheet={3}",
                        qry.HospId, qry.PatNo, qry.PatSeq, qry.ReturnSheet);
                    string response = ClientSend(mUrl);
                    if (response != "ERR")
                    {
                        Resp_IPDReturn resp = Newtonsoft.Json.JsonConvert.DeserializeObject<Resp_IPDReturn>(response);
                        if (resp.Succ) { return resp.Data; } else { return null; }
                    }
                    else { ApiCientFailedLog(mUrl); return null; }
                }
            }
            catch (Exception ex) { SysBaseServ.Log(_loginfo, ex, $"Api getIPDPatSeq({qry.ReturnSheet}...) 失敗:{ex}"); return null; }
        }

    }
}
