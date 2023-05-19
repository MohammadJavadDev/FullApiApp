using Common.Utilities;
using Entities;
using Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ApplicationDbContext :IdentityDbContext<User,Role,string>
    {
        public string _connectionString { get; set; }
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

            var entitiesAssembly = typeof(IEntity).Assembly;
            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            modelBuilder.AddRestrictDeleteBehaviorConvention();
           // modelBuilder.AddSequentialGuidForIdConvention();
            modelBuilder.AddSingularizingTableNameConvention();
        }

 

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

        public int ExecuteCommand(string query, object parameters = null)
        {
            int affectedRows = 0;
            using (var connection = new SqlConnection(Database.GetConnectionString()))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    if (parameters != null)
                    {
                        foreach (var prop in parameters.GetType().GetProperties())
                        {
                            command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters));
                        }
                    }
                    affectedRows = command.ExecuteNonQuery();
                }
            }
            return affectedRows;
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string query, object parameters = null) where TEntity : new()
        {
            var results = new List<TEntity>();
            using (var connection = new SqlConnection(Database.GetConnectionString()))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    if (parameters != null)
                    {
                        foreach (var prop in parameters.GetType().GetProperties())
                        {
                            command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters));
                        }
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entity = new TEntity();

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var propertyName = reader.GetName(i);
                                var property = entity.GetType().GetProperty(propertyName);

                                if (property != null && !reader.IsDBNull(i))
                                {
                                    var value = reader.GetValue(i);
                                    var propertyType = property.PropertyType;

                                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        propertyType = Nullable.GetUnderlyingType(propertyType);
                                    }

                                    if (propertyType.IsEnum)
                                    {
                                        var enumValue = (Enum)Enum.ToObject(propertyType, value);
                                        property.SetValue(entity, enumValue);
                                    }
                                    else
                                    {
                                        property.SetValue(entity, value);
                                    }
                                }
                            }

                            results.Add(entity);
                        }
                    }
                }
            }
            return results;
        }

    }
}
