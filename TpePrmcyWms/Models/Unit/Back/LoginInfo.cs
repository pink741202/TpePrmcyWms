using TpePrmcyWms.Models.DOM;
using TpePrmcyWms.Models.Service;
using Dapper;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using TpePrmcyWms.Models.Unit.Front;

namespace TpePrmcyWms.Models.Unit.Back
{
    public class LoginInfo
    {
        public LUser User { get; set; } //使用者基本資料(employee)
        public List<AuthCatelog> AuthDetail { get; set; } = new List<AuthCatelog>(); //取得每個目錄細部操作權限
        public List<MenuTree> Trees { get; set; } = new List<MenuTree>();
        public string FromIP { get; set; } = "";
        public LinkInfo Linkinfo { get; set; } = new LinkInfo();
        public List<int> CbntAuth { get; set; } = new List<int>(); //可使用的櫃子權限



        public bool set(employee emp, int AtCbntFid)
        {
            try
            {
                User = new LUser()
                {
                    Fid = emp.FID,
                    Name = emp.name,
                    Acct = emp.empacc,
                    RoleFid = emp.RoleFid,
                    dptFid = emp.dptFid,
                    comFid = emp.comFid,
                    SyncAuth = (emp.SyncAsRole) ? emp.RoleFid ?? 0 : emp.FID,
                    pagesize = emp.pagesize
                };
                using (DBcPharmacy db = new DBcPharmacy())
                {
                    #region 權限
                    //if (emp.SyncAsRole)
                    //{
                    //    AuthDetail = db.AuthCatelog.Where(x => x.RoleFid == User.RoleFid && x.Queryable).ToList();
                    //}
                    //else
                    //{
                    //    AuthDetail = db.AuthCatelog.Where(x => x.EmpFid == User.Fid && x.Queryable).ToList();
                    //}

                    //聯醫改個人權限,並加入每個櫃子功能
                    int from_config = Convert.ToInt32(SysBaseServ.JsonConf("TestEnvironment:ImaginaryKioskCbntFid"));
                    List<UserCbntFnAuth> auths = db.UserCbntFnAuth.Where(x => x.EmpFid == User.Fid && x.Active).ToList();
                    if(AtCbntFid > 0) { auths.Add(new UserCbntFnAuth { MnLFid = 24, EmpFid = User.Fid, CbntFid = AtCbntFid }); }
                    foreach (var item in auths.Where(x => x.CbntFid == AtCbntFid || x.CbntFid == (from_config > 0 ? 0 : -9))) //前後台權限
                    {
                        AuthDetail.Add(new AuthCatelog
                        {
                            EmpFid = item.EmpFid, 
                            MenuLFid = item.MnLFid, 
                            Queryable = true, 
                            Creatable = true,
                            Updatable = true, 
                            Deletable = true,
                        });
                    }

                    CbntAuth = auths.GroupBy(x => x.CbntFid).Select(x=>x.Key).ToList();
                    #endregion
                }
                #region 有權限的目錄
                using (SqlConnection conn = new SqlConnection(SysBaseServ.JsonConfConnString("TpePrmcyWms")))
                {
                    string sql = "select ml.MnTFid MtFid, ml.fid as L1fid, ml2.fid as L2fid, mt.MenuName MtName, " +
                    "ml.CatelogName as L1Name ,ml2.CatelogName as L2Name,  " +
                    "mt.link as MtLink, ml.Link as L1Link, ml2.link as L2Link, " +
                    "mt.System, ml2.OnDisplay L2Display " +
                    "from MenuLeft ml  " +
                    "left join MenuTop mt on mt.FID = ml.MnTFid  " +
                    "left join MenuLeft ml2 on ml.fid = ml2.upFID and ml2.LAYER = 2 " +
                    "where isnull(ml.MnTFid,'') <> '' and ml.LAYER = 1  " + //and mt.SYSTEM = 'BACKEND'
                    $" and (ml2.fid in ({qwServ.ToSqlString(AuthDetail.Select(x => x.MenuLFid).ToList(), ",")}) " +
                    $" or ml.fid in ({qwServ.ToSqlString(AuthDetail.Select(x => x.MenuLFid).ToList(), ",")}) ) " +
                    "order by mt.Sorting, ml.MnTFid, ml.sorting, ml.fid, ml2.sorting ";
                    Trees = conn.Query<MenuTree>(sql).ToList();

                    //檢查 
                    foreach (MenuTree t in Trees)
                    {
                        if (!string.IsNullOrEmpty(t.L2Link)) { if (t.L2Link.IndexOf(',') < 0) { t.L2Link += ",Index"; } }
                        if (!string.IsNullOrEmpty(t.L1Link)) { if (t.L1Link.IndexOf(',') < 0) { t.L1Link += ",Index"; } }
                        if (!string.IsNullOrEmpty(t.L1Link) && string.IsNullOrEmpty(t.L2Link))
                        {
                            t.L2Link = t.L1Link;
                            t.L2fid = t.L1fid;
                            t.L2Name = t.L1Name;
                        }
                    }
                }
                #endregion
                #region 目前頁面
                Linkinfo = new LinkInfo();
                #endregion
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }


    public class LUser
    {
        public int Fid { set; get; }
        public string Name { set; get; }
        public string Acct { set; get; }
        public int? RoleFid { set; get; }
        public int? dptFid { set; get; }
        public int? comFid { set; get; }
        public int pagesize { set; get; } = 20;
        public int SyncAuth { set; get; }

    }

    public class MenuTree
    {
        public int MtFid { get; set; }
        public int L1fid { get; set; }
        public int L2fid { get; set; }
        public string MtName { get; set; }
        public string L1Name { get; set; }
        public string L2Name { get; set; }
        public string MtLink { get; set; }
        public string L1Link { get; set; }
        public string L2Link { get; set; }
        public string System { get; set; }
        public bool L2Display { get; set; }
    }

    public class LinkInfo
    {
        public int MenuLFid { get; set; }
        public string ActionName { get; set; } = "";
        public string ContrlName { get; set; } = "";
        public string Method { get; set; } = "";

    }
}
