using System;
using System.IO; 
using System.Text;
using System.Globalization;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Dapper;
using Microsoft.Data.SqlClient;
using TpePrmcyWms.Models.Unit.Back;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using System.Drawing.Text;
using TpePrmcyWms.Models.DOM;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using NuGet.Packaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json;

namespace TpePrmcyWms.Models.Service
{
    static class ViewFunc
    {
        #region 日期呈現
        public static string vDateFormat(DateTime? value, string format)
        {
            if (value == null) { return ""; }
            return ((DateTime)value).ToString(format);
        }
        #endregion

        #region 查對應值 Fid=Value Map Dictionary        
        public static Dictionary<string, string> MapFidToName<T>(T obj)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var p in obj.GetType().GetProperties())
            {
                if ((p.Name.ToLower().EndsWith("fid") && p.Name.ToLower() != "fid") || p.Name.ToLower() == "modid")
                {
                    int value = (int?)p.GetValue(obj, null) ?? 0;
                    if (value == 0) { result.Add(p.Name, ""); continue; }
                    using (DBcPharmacy db = new DBcPharmacy())
                    {
                        try
                        {
                            switch (p.Name.ToLower())
                            {
                                case "comfid":
                                    result.Add(p.Name, db.Company.Find(value)?.comtitle ?? "");
                                    result.Add(p.Name + "2", db.Company.Find(value)?.comsttitle ?? "");
                                    break;
                                case "dptfid":
                                    result.Add(p.Name, db.Department.Find(value)?.dpttitle ?? "");
                                    break;
                                case "empfid":
                                case "modfid":
                                case "modid":
                                    result.Add(p.Name, db.employee.Find(value)?.name ?? "");
                                    break;
                                case "rolefid":
                                    result.Add(p.Name, db.AuthRole.Find(value)?.RoleName ?? "");
                                    break;
                                case "cbntfid":
                                    result.Add(p.Name, db.Cabinet.Find(value)?.CbntName ?? "");
                                    break;
                                case "drugfid":
                                    result.Add(p.Name, $"{db.DrugInfo.Find(value)?.DrugCode ?? ""}-{db.DrugInfo.Find(value)?.DrugName ?? ""}");
                                    break;
                            }
                        }
                        catch
                        {
                            result.Add(p.Name, "");
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region 查對應值 ParamaterOptions        
        public static List<vParamOptions> vParamSelect(List<string> qgroups)
        {
            List<vParamOptions> result = new List<vParamOptions>();
            using (DBcPharmacy db = new DBcPharmacy())
            {                
                result = db.ParamOption.Where(x => qgroups.Contains(x.GroupCode))
                    .OrderBy(x=>x.GroupCode).ThenBy(x => x.Sorting)
                    .Select(x =>
                    new vParamOptions()
                    {
                        Group = x.GroupCode,
                        Value = x.OptionCode,
                        Text = x.OptionName,
                    }).ToList();
            }
            return result;
        }
        public static List<vParamOptions> vParamSelect()
        {
            List<vParamOptions> result = new List<vParamOptions>();
            using (DBcPharmacy db = new DBcPharmacy())
            {
                result = (from item in db.ParamOption
                          where item.SysType != "BaseSys"
                          group item by new { item.GroupCode, item.GroupName } into grouped
                          select new vParamOptions()
                          {
                              Group = "Grouped",
                              Value = grouped.Key.GroupCode,
                              Text = grouped.Key.GroupName
                          }).ToList();
            }
            return result;
        }
        #endregion

        #region 下拉用清單 Fid=Value, Name=Text         
        public static List<vSelectOptions> SelectFidToValue(List<string> qlist)
        {
            List<vSelectOptions> result = new List<vSelectOptions>();
            foreach (string item in qlist)
            {
                List<string> param = item.Split(':').ToList();
                string q = param[0];
                int cbntFid = 0;

                using (DBcPharmacy db = new DBcPharmacy())
                {
                    List<vSelectOptions> query = new List<vSelectOptions>();
                    switch (q.ToLower())
                    {
                        case "company":
                            query = (from o in db.Company
                                     select new vSelectOptions() { Group = q, Value = o.FID.ToString(), Text = o.comsttitle }).ToList();
                            break;
                        case "department":
                            query = (from o in db.Department
                                     select new vSelectOptions() { Group = q, Value = o.FID.ToString(), Text = o.dpttitle, UpGroup = db.Company.Where(x=>x.FID == o.comFid).First().comsttitle }).ToList();
                            break;
                        case "employee":
                            query = (from o in db.employee
                                     select new vSelectOptions() { Group = q, Value = o.FID.ToString(), Text = o.name }).ToList();
                            break;
                        case "authrole":
                            query = (from o in db.AuthRole
                                     select new vSelectOptions() { Group = q, Value = o.FID.ToString(), Text = o.RoleName }).ToList();
                            break;
                        case "menuleftlink":
                            query = (from o in db.MenuLeft
                                     where o.Link != ""
                                     select new vSelectOptions() { Group = q, Col = o.Link.Replace(",List", ""), Text = o.CatelogName }).ToList();
                            break;
                        case "cabinet":
                            query = (from o in db.Cabinet
                                     select new vSelectOptions() { Group = q, Value = o.FID.ToString(), Text = o.CbntName }).ToList();
                            break;
                        case "drawerno":
                            using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
                            {
                                string sql = $"select '{q}' [Group], a.FID Value, CbntName+'#'+convert(varchar,No) Text " +
                                    "from Drawers a " +
                                    "left join Cabinet b on a.CbntFid = b.FID ";
                                query = conn.Query<vSelectOptions>(sql).ToList();
                            }
                            break;
                        case "drawernosub":
                            using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
                            {
                                string sql = $"select '{q}' [Group], CbntName UpGroup, a.FID Value, '#'+ convert(varchar,No) Text " +
                                    "from Drawers a " +
                                    "left join Cabinet b on a.CbntFid = b.FID ";
                                query = conn.Query<vSelectOptions>(sql).ToList();
                            }
                            break;
                        case "drugcode":
                            query = (from o in db.DrugInfo
                                     select new vSelectOptions() { Group = q, Value = o.FID.ToString(), Text = o.DrugCode }).ToList();
                            break;
                        case "drugname":
                            query = (from o in db.DrugInfo
                                     select new vSelectOptions() { Group = q, Value = o.FID.ToString(), Text = o.DrugName }).ToList();
                            break;
                        case "drugfullquery": //drugcodename
                            query = (from o in db.DrugInfo
                                     where !o.DrugCode.Contains("_")
                                     select new vSelectOptions() { Group = q, Value = $"{o.DrugCode}，{o.DrugName}，{o.FID}", Text = $"" }).ToList();
                            foreach(var ls in query)
                            {
                                int fid = Convert.ToInt32(ls.Value.Split("，")[2]);
                                List<string?> barcodes = db.DrugPackage.Where(x => x.DrugFid == fid && !string.IsNullOrEmpty(x.BarcodeNo)).Select(x => x.BarcodeNo).ToList();
                                ls.Text = string.Join(",", barcodes);
                            }
                            break;
                        case "drugfullqueryincabinet": //drugcodenameincabinet
                            //需要參數 cabinetfid                            
                            try { cbntFid = Convert.ToInt32(param[1]); } catch (Exception ex) { }
                            bool hasReplaceDrug = param.Count >= 3 && param[2] == "replacedrug";
                            try
                            {
                                var drugincbnt = db.DrugGrid.Where(x => x.CbntFid == cbntFid).Select(x => x.DrugFid).ToList();
                                query = (from o in db.DrugInfo
                                         where drugincbnt.Contains(o.FID) && !o.DrugCode.Contains("_")
                                            && (!hasReplaceDrug ? ((o.ReplaceTo ?? 0) == 0) : true)
                                         select new vSelectOptions() { Group = q, Value = $"{o.DrugCode}，{o.DrugName}，{o.FID}", Text = $"" }).ToList();
                                foreach (var ls in query)
                                {
                                    int fid = Convert.ToInt32(ls.Value.Split("，")[2]);
                                    List<string?> barcodes = db.DrugPackage.Where(x => x.DrugFid == fid && !string.IsNullOrEmpty(x.BarcodeNo)).Select(x => x.BarcodeNo).ToList();
                                    ls.Text = string.Join(",", barcodes);
                                }
                            }
                            catch (Exception ex) { }
                            break;
                        case "drugfullqueryoffsetincabinet": //drugcodenameoffsetincabinet
                            //需要參數 cabinetfid                            
                            try { cbntFid = Convert.ToInt32(param[1]); } catch (Exception ex) { }                            
                            try
                            {
                                var drugincbnt = db.DrugGrid.Where(x => x.CbntFid == cbntFid && (x.OffsetActive ?? false)).Select(x => x.DrugFid).ToList();
                                query = (from o in db.DrugInfo
                                         where drugincbnt.Contains(o.FID) && !o.DrugCode.Contains("_")                                             
                                         select new vSelectOptions() { Group = q, Value = $"{o.DrugCode}，{o.DrugName}，{o.FID}", Text = $"" }).ToList();
                                foreach (var ls in query)
                                {
                                    int fid = Convert.ToInt32(ls.Value.Split("，")[2]);
                                    List<string?> barcodes = db.DrugPackage.Where(x => x.DrugFid == fid && !string.IsNullOrEmpty(x.BarcodeNo)).Select(x => x.BarcodeNo).ToList();
                                    ls.Text = string.Join(",", barcodes);
                                }
                            }
                            catch (Exception ex) { }
                            break;
                        case "drugfullqueryisvax":  //drugcodenameisvax
                            //需要參數 cabinetfid
                            try { cbntFid = Convert.ToInt32(param[1]); } catch (Exception ex) { }
                            try
                            {
                                query = (from o in db.DrugInfo
                                         where o.isVax == true && ((o.ReplaceTo ?? 0) == 0)
                                         select new vSelectOptions() { Group = q, Value = $"{o.DrugCode}，{o.DrugName}，{o.FID}", Text = $"" }).ToList();
                                foreach (var ls in query)
                                {
                                    int fid = Convert.ToInt32(ls.Value.Split("，")[2]);
                                    List<string?> barcodes = db.DrugPackage
                                        .Where(x => x.DrugFid == fid && !string.IsNullOrEmpty(x.BarcodeNo))
                                        .Select(x => x.BarcodeNo).ToList();
                                    ls.Text = string.Join(",", barcodes);
                                }
                            }
                            catch (Exception ex) { }
                            break;
                        case "drgridsub":
                            using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
                            {
                                string sql = $"select '{q}' [Group], CbntName+'#'+convert(varchar,No) UpGroup, a.FID Value, DrugName Text " +
                                    "from DrugGrid a " +
                                    "left join Cabinet b on a.CbntFid = b.FID " +
                                    "left join Drawers c on a.DrawFid = c.FID " +
                                    "left join DrugInfo d on a.DrugFid = d.FID ";
                                query = conn.Query<vSelectOptions>(sql).ToList();
                            } 
                            break;
                        case "drugweigh":
                            using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
                            {
                                string sql = $"select '{q}' [Group], di.DrugName UpGroup, dp.Fid Value " +
                                    ", convert(varchar, UnitWeight)+'g / '+convert(varchar, UnitQty)+' '+UnitTitle Text " +
                                    "from DrugPackage dp " +
                                    "left join DrugInfo di on dp.DrugFid = di.FID ";
                                query = conn.Query<vSelectOptions>(sql).ToList();
                            }
                            break;
                        case "cabinetsub":
                            using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
                            {
                                string sql = $"select '{q}' [Group], b.comsttitle UpGroup, a.FID Value, CbntName Text " +
                                    "from Cabinet a " +
                                    "left join Company b on a.comFid = b.FID ";
                                query = conn.Query<vSelectOptions>(sql).ToList();
                            }
                            break;
                    }
                    result.AddRange(query);
                }
            }
            return result;
        }
        
        #endregion

        #region 查對應值(用下拉物件找) 怕有null或找不到,用這比較保險
        public static string vFindValue(List<vSelectOptions> optlist, string group, string key)
        {
            if (key == null) { return ""; }
            vSelectOptions s = optlist.Where(x => x.Group == group && x.Col == key).FirstOrDefault();
            return s == null ? "" : s.Text;
        }
        #endregion
    }

    #region 下拉用物件
    public class vSelectOptions
    {
        public string Group { get; set; }
        public string Value { get; set; }
        public string Col { get; set; }
        public string Text { get; set; }
        public string UpGroup { get; set; }
    }    
    public class vParamOptions
    {
        public string Group { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
    }
    #endregion
}
