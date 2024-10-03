using Microsoft.EntityFrameworkCore;
using TpePrmcyWms.Models.Service;
namespace TpePrmcyWms.Models.DOM
{
    public partial class DBcPharmacy : DbContext
    {
        public IConfiguration configuration { get; }
        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            opt.UseSqlServer(SysBaseServ.JsonConfConnString("TpePrmcyWms"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //插入資料列時沒有該資料行的值，則會使用預設值
            //ref: https://learn.microsoft.com/zh-tw/ef/core/modeling/generated-properties?tabs=data-annotations
            modelBuilder.Entity<employee>().Property(b => b.emptype).HasDefaultValue("1");
            modelBuilder.Entity<employee>().Property(b => b.empstatus).HasDefaultValue("1");



        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DrugInfo> DrugInfo { get; set; }
        public virtual DbSet<DrugGrid> DrugGrid { get; set; }
        public virtual DbSet<DrugPickLog> DrugPickLog { get; set; }
        public virtual DbSet<DrugPackage> DrugPackage { get; set; }
        public virtual DbSet<employee> employee { get; set; }
        public virtual DbSet<ParamOption> ParamOption { get; set; }
        public virtual DbSet<AuthRole> AuthRole { get; set; }
        public virtual DbSet<AuthCatelog> AuthCatelog { get; set; }
        public virtual DbSet<ScaleWeighQtyLog> ScaleWeighQtyLog { get; set; }
        public virtual DbSet<SensorDevice> SensorDevice { get; set; }
        public virtual DbSet<StockBill> StockBill { get; set; }
        public virtual DbSet<Cabinet> Cabinet { get; set; }
        public virtual DbSet<MenuLeft> MenuLeft { get; set; }
        public virtual DbSet<MenuTop> MenuTop { get; set; }
        public virtual DbSet<OperateLog> OperateLog { get; set; }
        public virtual DbSet<PrscptBill> PrscptBill { get; set; }
        public virtual DbSet<MapPackOnSensor> MapPackOnSensor { get; set; }
        public virtual DbSet<Drawers> Drawers { get; set; }
        public virtual DbSet<SensorComuQuee> SensorComuQuee { get; set; }
        public virtual DbSet<AlertNotification> AlertNotification { get; set; }
        public virtual DbSet<MethadonBill> MethadonBill { get; set; }
        public virtual DbSet<MapPrscptOnBill> MapPrscptOnBill { get; set; }
        public virtual DbSet<MapMenuOnCbnt> MapMenuOnCbnt { get; set; }
        public virtual DbSet<VaxSkd> VaxSkd { get; set; }
        public virtual DbSet<VaxSkdDtl> VaxSkdDtl { get; set; }
        public virtual DbSet<MapDrugFreqOnCode> MapDrugFreqOnCode { get; set; }
        public virtual DbSet<DrugGridBatchNo> DrugGridBatchNo { get; set; }
        public virtual DbSet<UserCbntFnAuth> UserCbntFnAuth { get; set; }
        public virtual DbSet<DrugLimitedTo> DrugLimitedTo { get; set; }
        public virtual DbSet<BalanceDiffFile> BalanceDiffFile { get; set; }

        
    }
}
