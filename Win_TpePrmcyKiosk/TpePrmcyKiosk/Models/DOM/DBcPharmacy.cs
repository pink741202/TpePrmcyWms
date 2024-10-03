using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TpePrmcyWms.Models.DOM;

namespace TpePrmcyKiosk.Models.DOM
{   
    
    public class DBcPharmacy : DbContext //DbContext ref:MEFCore.Design
    {
        private string ConnStr = ConfigurationManager.AppSettings["ConnectString"];
        //public orContext(DbContextOptions<orContext> o) : base(o) { }

        //Cross DB code here or in startup.cs
        protected override void OnConfiguring(DbContextOptionsBuilder optBuder)
        {
            optBuder.UseSqlServer(ConnStr);
        }

        public DbSet<Cabinet>? Cabinet { get; set; } 
        public DbSet<Drawers>? Drawers { get; set; } 
        public DbSet<DrugInfo>? DrugInfo { get; set; } 
        public DbSet<DrugGrid>? DrugGrid { get; set; } 
        public DbSet<DrugPackage>? DrugPackage { get; set; } 
        public DbSet<MapPackOnSensor>? MapPackOnSensor { get; set; } 
        public DbSet<ScaleWeighQtyLog>? ScaleWeighQtyLog { get; set; } 
        public DbSet<SensorComuQuee>? SensorComuQuee { get; set; } 
        public DbSet<SensorDevice>? SensorDevice { get; set; } 
        public DbSet<AlertNotification>? AlertNotification { get; set; } 
        public DbSet<employee>? employee { get; set; } 
        





    }
    
}
