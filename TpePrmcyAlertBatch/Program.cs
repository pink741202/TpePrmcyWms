using TpePrmcyAlertBatch.Models.DOM;
using TpePrmcyWms.Models.DOM;
using TpePrmcyAlertBatch.Models.Unit;
using TpePrmcyAlertBatch.Models.Service;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using System.Web;
using System.Xml;
using Newtonsoft.Json;

Console.WriteLine("Begin");

DBcPharmacy _db = new DBcPharmacy();

List<ParamOption> confTimes = _db.ParamOption.Where(x => x.GroupCode == "TpePrmcyAlertBatch").ToList();
if (confTimes.Count() < 3)
{
    qwFunc.logshow("取得[聯醫批次參數(ParamOption)]時間參數錯誤"); return;
}
List<DateTime> FlagTimes = new List<DateTime>();
List<bool> ifRun = new List<bool>() { false, false, false }; //0:each Day 1:each Hour 2:抓結存量差異分析統計檔案

string sExecTime = confTimes.Where(x => x.OptionCode == "ExecTime").FirstOrDefault()?.OptionName ?? "err" ;
string sExecTime_EachDay = confTimes.Where(x => x.OptionCode == "ExecTime_EachDay").FirstOrDefault()?.OptionName ?? "err" ;
string sExecTime_EachHour = confTimes.Where(x => x.OptionCode == "ExecTime_EachHour").FirstOrDefault()?.OptionName ?? "err" ;
string sExecTime_BlnsDifFile = confTimes.Where(x => x.OptionCode == "ExecTime_BlnsDifFile").FirstOrDefault()?.OptionName ?? "err" ;
if(sExecTime == "err" || sExecTime_EachDay == "err" || sExecTime_EachHour == "err" )
{
    qwFunc.logshow("取得某項時間參數時出現錯誤"); return;
}

DateTime ExecTime = new DateTime();
DateTime ExecTime_EachDay = new DateTime();
DateTime ExecTime_EachHour = new DateTime();
DateTime ExecTime_BlnsDifFile = new DateTime();
DateTime now = DateTime.Now;
try
{
    
    ExecTime = sExecTime == "" ? now.AddDays(-5) : Convert.ToDateTime(sExecTime);
    ExecTime_EachDay = sExecTime_EachDay == "" ? now : Convert.ToDateTime(sExecTime_EachDay);
    ExecTime_EachHour = sExecTime_EachHour == "" ? now : Convert.ToDateTime(sExecTime_EachHour);
    ExecTime_BlnsDifFile = sExecTime_BlnsDifFile == "" ? now : Convert.ToDateTime(sExecTime_BlnsDifFile);
    FlagTimes.Add(ExecTime);
    FlagTimes.Add(now);

    if(sExecTime_EachDay == "") { ifRun[0] = true; }
    else
    {
        ifRun[0] = ExecTime_EachDay < FlagTimes[1].Date ;
    }
    if (sExecTime_EachHour == "") { ifRun[1] = true; }
    else
    {
        ifRun[1] = ExecTime_EachHour < FlagTimes[1].AddMinutes(-FlagTimes[1].Minute);
    }
    if (sExecTime_BlnsDifFile == "") { ifRun[2] = true; }
    else
    {
        DateTime time1 = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
        DateTime time2 = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0);
        ifRun[2] = (now >= time1 && now < time2 && ExecTime_BlnsDifFile < time1); //8點的未執行        
        ifRun[2] = (now >= time2 && ExecTime_BlnsDifFile < time2); //15點的未執行
    }
}
catch(Exception ex)
{
    qwFunc.logshow("取得時間參數時，轉換時間型態出現錯誤"); return;
}

ParamOption po1 = confTimes.Where(x => x.OptionCode == "ExecTime").FirstOrDefault();
po1.OptionName = now.ToString("yyyy/MM/dd HH:mm:ss");
_db.ParamOption.Update(po1);
_db.SaveChanges();
if (ifRun[0])
{
    ParamOption po2 = confTimes.Where(x => x.OptionCode == "ExecTime_EachDay").FirstOrDefault();
    po2.OptionName = now.ToString("yyyy/MM/dd HH:mm:ss");
    _db.ParamOption.Update(po2);
    _db.SaveChanges();
}
if (ifRun[1])
{
    ParamOption po3 = confTimes.Where(x => x.OptionCode == "ExecTime_EachHour").FirstOrDefault();
    po3.OptionName = now.ToString("yyyy/MM/dd HH:mm:ss");
    _db.ParamOption.Update(po3);
    _db.SaveChanges();
}
if (ifRun[2])
{
    ParamOption po4 = confTimes.Where(x => x.OptionCode == "ExecTime_BlnsDifFile").FirstOrDefault();
    po4.OptionName = now.ToString("yyyy/MM/dd HH:mm:ss");
    _db.ParamOption.Update(po4);
    _db.SaveChanges();
}


//AlertBatchService alertserv = new AlertBatchService(FlagTimes, ifRun);

//取檔案
if (ifRun[2])
{
    HsptlApiBatchService api = new HsptlApiBatchService();
    api.saveBalanceDiffFile(now);
}