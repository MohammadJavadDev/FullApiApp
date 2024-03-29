﻿using Common.Utilities;
using Entities;
using Entities.CRM;
using Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options):base(options)
        {
            
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    optionsBuilder.UseSqlServer("");
        //    base.OnConfiguring(optionsBuilder);
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>().ToTable("Contact", schema: "itnj");
            modelBuilder.Entity<ContactPSPDocument>().ToTable("ContactPSPDocument", schema: "itnj");
            modelBuilder.Entity<ContactPSPDocumentUrl>().ToTable("ContactPSPDocumentUrl", schema: "itnj");
            modelBuilder.Entity<Reception>().ToTable("Reception", schema: "itnj");
            modelBuilder.Entity<PSPDocument>().ToTable("PSPDocument", schema: "itnj");
            modelBuilder.Entity<UserCrm>().ToTable("User",schema: "security");
            modelBuilder.Entity<vw_MultiContactPspAccount>().ToView("vw_MultiContactPspAccount", schema: "itnj")
                .HasNoKey();
            

            

            var entitiesAssembly = typeof(IEntity).Assembly;
          //  modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
          //  modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
           // modelBuilder.AddRestrictDeleteBehaviorConvention();
           // modelBuilder.AddSequentialGuidForIdConvention();
        //    modelBuilder.AddSingularizingTableNameConvention();
        }

        //public class ApplicationDbContextDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        //{
        //    public ApplicationDbContext CreateDbContext(string[] args)
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //        optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=NewDb; User Id=sa; Password=Aa123456 ;TrustServerCertificate=True");

        //        return new ApplicationDbContext(optionsBuilder.Options);
        //    }
        //}

        public override int SaveChanges()
        {
            _cleanString();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _cleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cleanString(); 
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(c=>c.State == EntityState.Added
                ||c.State == EntityState.Modified);
            foreach(var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance)
                    .Where(c => c.CanRead && c.CanWrite && c.PropertyType == typeof(string));

                foreach(var property in properties)
                {
                    var propName = property.Name;
                    string? val = (string)property.GetValue(item.Entity, null);

                    if(val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                       property.SetValue(item.Entity, newVal, null);

                    }
                }
            }


        }
 
    }
}
