using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MJ.EntityFramwork.Context
{
    public interface IDbContext : IDisposable
    {
        IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery) where TEntity : new();
        IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery, object parameters) where TEntity : new();
        string Where1<TEntity>(string tableName ,string schema, Expression<Func<TEntity, bool>> predicate) where TEntity : new();
        int ExecuteNonQuery(string sqlQuery);
        int ExecuteCommand(string sqlQuery, object parameters);
    }
}
