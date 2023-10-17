using Entities.CRM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class NewCrmDbContext : DbContext
    {

        public NewCrmDbContext() : base(GetOptions())
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ContactPSPDocument>().ToTable("ContactPSPDocument", schema: "itnj");
            modelBuilder.Entity<ConvertLog>().ToTable("ConvertLog", schema: "pub");
        }

        public DbSet<ContactPSPDocument> ContactPSPDocuments { get; set; }
        public DbSet<ConvertLog> ConvertLogs { get; set; }


        private static DbContextOptions GetOptions()
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), "Server=192.168.202.1;Database=Vida;User ID=crm;Password=Crm@@987;TrustServerCertificate=True").Options;
        }
    }
}
