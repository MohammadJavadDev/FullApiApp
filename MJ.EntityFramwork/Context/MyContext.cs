 
using Entities.Users;
 
using System.Collections;
 
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
 
using System.Linq.Expressions;
using System.Reflection;
 
public interface IDbContext
{
    int ExecuteCommand(string query, object parameters = null);
    object Execute(Expression expression, bool isEnumerable);
    IEnumerable<T> ExecuteQuery<T>(string query, object parameters = null) where T : new();
 
}

public class DbContext : IDbContext  
{
    private readonly string _connectionString;
 

    public DbContext(string connectionString)
    {
        _connectionString = connectionString;
        Users = new DbSet<User>(this);
    }

    public int ExecuteCommand(string query, object parameters = null)
    {
        int affectedRows = 0;
        using (var connection = new SqlConnection(_connectionString))
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
        using (var connection = new SqlConnection(_connectionString))
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
    public IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery) where TEntity : new()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand(sqlQuery, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var results = new List<TEntity>();

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

                    return results;
                }
            }
        }
    }

    public object Execute(Expression expression, bool isEnumerable)
    {
        var d  = expression.ToSql<User>();
        return d;
    }

    public DbSet<User> Users { get; set; }
}

public interface IDbSet<TEntity> where TEntity : new()
{
    IEnumerable<TEntity> GetAll();
    TEntity GetById(int id);
    void Insert(TEntity entity);
    void Update(TEntity entity);
    void Delete(int id);
}

 

public class DbSet<TEntity> : IDbSet<TEntity> where TEntity : new()
{
    private readonly IDbContext _context;
    private readonly string _tableName;
    private readonly string _tableNameAliase;
    private readonly string _schemaName;
    public IQueryable<TEntity> _entities { get; set; } 

    public DbSet(IDbContext context )
    {
        _context = context;

        _tableName =   typeof(TEntity).Name;
        _schemaName = "dbo";
        var attrTable = typeof(TEntity).GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;
        if (attrTable != null)
        {
            _tableName = attrTable.Name ?? _tableName;
            _schemaName = attrTable.Schema ?? _schemaName;
            _tableNameAliase = _tableName.ToLower().Substring(0, 2);
        }

        _entities = new Queryable<TEntity>(_context);
    }

    public IEnumerable<TEntity> GetAll()
    {
        var query = $"SELECT * FROM {_tableName}";
        return _context.ExecuteQuery<TEntity>(query);
    }

    public TEntity GetById(int id)
    {
        var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
        return _context.ExecuteQuery<TEntity>(query, new { Id = id }).SingleOrDefault();
    }

    public void Insert(TEntity entity)
    {
        var properties = typeof(TEntity).GetProperties();
        var propertyNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));
        var query = $"INSERT INTO {_tableName} ({propertyNames}) VALUES ({parameterNames})";
        _context.ExecuteCommand(query, entity);
    }

    public void Update(TEntity entity)
    {
        var properties = typeof(TEntity).GetProperties().Where(p => p.Name != "Id"); // exclude the primary key
        var propertySetters = properties.Select(p => $"{p.Name} = @{p.Name}");
        var query = $"UPDATE {_tableName} SET {string.Join(", ", propertySetters)} WHERE Id = @Id";
        _context.ExecuteCommand(query, entity);
    }

    public void Delete(int id)
    {
        var query = $"DELETE FROM {_tableName} WHERE Id = @Id";
        _context.ExecuteCommand(query, new { Id = id });
    }


   public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
       
        return  _entities.Where(predicate);
    }


    public IQueryable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
    
        return  _entities.Select(selector);
    }

 
    public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        var query = $"SELECT TOP 1 * FROM {_tableName} WHERE {predicate.ToSql(_tableName)}";
        return _context.ExecuteQuery<TEntity>(query, predicate.Parameters).FirstOrDefault();
    }

    public TEntity FirstOrDefault()
    {
        var query = $"SELECT TOP 1 * FROM {_tableName}";
        return _context.ExecuteQuery<TEntity>(query).FirstOrDefault();
    }

    public List<TEntity> ToList()
    {
        
        return  _entities.ToList();
    }
}


public static class ExpressionExtensions
{
    public static string ToSql<TEntity>(this Expression predicate)
    {

        var d = new SqlVisitor<TEntity>();
        d.Visit
            (predicate);
        return d.WhereConditions[0];
    }

    public static string ToSql<TEntity, TResult>(this Expression<Func<TEntity, TResult>> selector, string tableName)
    {
        return new SqlVisitor<TEntity>().Visit(selector.Body).ToString();
    }
}
public class SqlVisitor<TEntity> : ExpressionVisitor
{
    private readonly List<string> _selectColumns = new List<string>();
    private readonly List<string> _joinTables = new List<string>();
    private readonly Stack<string> _whereConditions = new Stack<string>();

        public SqlVisitor(  )
        {
            _tableName = nameof(TEntity);
            _tableNameAliase = _tableName.ToLower().Substring(0, 2);
        }

        private string _tableName;
        private readonly string _tableNameAliase;


        public string[] SelectColumns
    {
        get { return _selectColumns.ToArray(); }
    }

    public string[] JoinTables
    {
        get { return _joinTables.ToArray(); }
    }

    public string[] WhereConditions
    {
        get { return _whereConditions.ToArray(); }
    }

    public string ToSql()
    {
        var sql = $"SELECT {string.Join(", ", _selectColumns)} FROM {_tableName}";
        if (_joinTables.Count > 0)
        {
            sql += " " + string.Join(" ", _joinTables);
        }
        if (_whereConditions.Count > 0)
        {
            sql += " WHERE " + string.Join(" AND ", _whereConditions);
        }
        return sql;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (_tableName == null)
        {
            _tableName = node.Type.Name;
        }
        return base.VisitParameter(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
   
 
           if (node.Expression is ParameterExpression parameter)
          {
 
           _whereConditions.Push($"[{_tableNameAliase}].[{node.Member.Name}]" );
            
            return node;
        }
        else if (node.Expression is MemberExpression member)
        {
            if (member.Expression is ParameterExpression parameter2)
            {
                var columnName1 = GetColumnName(node.Member.Name);
                var columnName2 = GetColumnName(member.Member.Name);
                if (!_joinTables.Contains($"JOIN {columnName2} ON {columnName1}.{columnName2}"))
                {
                    _joinTables.Add($"JOIN {columnName2} ON {columnName1}.{columnName2}");
                }
                return node;
            }
        }
        throw new NotSupportedException($"Member '{node.Member.Name}' is not supported.");
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {


             

       if (node.Left is ConstantExpression)
            {
                Visit(node.Left);
                Visit(node.Right);
                 
            }
            else if (node.Right is ConstantExpression)
            {
                Visit(node.Right);
                Visit(node.Left);
            }
            else
            {
                Visit(node.Left);
                Visit(node.Right);
            }
        

          
        var op = GetOperator(node.NodeType);

       var stringQuery = $"({_whereConditions.Pop()} {op} {_whereConditions.Pop()})";
           
      _whereConditions.Push(stringQuery);
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
       
        var value = node.Value;
 
            if (value == null)
        {
            _whereConditions.Push("NULL");
        }
        else if (value is string || value is char)
        {
            _whereConditions.Push($"'{value}'");
        }
        else if (value is bool)
        {
            _whereConditions.Push((bool)value ? "1" : "0");
        }
        else if (value is byte || value is sbyte || value is short || value is ushort ||
                 value is int || value is uint || value is long || value is ulong ||
                 value is float || value is double || value is decimal)
        {
            _whereConditions.Push(value.ToString());
        }
        else
        {
             
             
            throw new NotSupportedException($"Constant of type '{value.GetType().Name}' is not supported.");
        }
        return node;
    }

        private static string GetColumnName(string memberName)
        {
            return memberName.ToLower();
        }
        private static string GetOperator(ExpressionType nodeType)
            {
                switch (nodeType)
                {
                    case ExpressionType.Equal:
                        return " = ";
                    case ExpressionType.NotEqual:
                        return " <> ";
                    case ExpressionType.LessThan:
                        return " < ";
                    case ExpressionType.LessThanOrEqual:
                        return " <= ";
                    case ExpressionType.GreaterThan:
                        return " > ";
                    case ExpressionType.GreaterThanOrEqual:
                        return " >= ";
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        return " AND ";
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        return " OR ";
                    default:
                        throw new NotSupportedException($"Operator '{nodeType}' is not supported.");
                }
            }
 
            protected override Expression VisitMethodCall(MethodCallExpression node)
        {

            if (node.Method.Name == "Contains")
            {
                string columnName = null;
                var constantValue = GetConstantValue(node.Arguments.First());

                if (node.Arguments.Count > 1 && node.Arguments[1] is MemberExpression memberExpression)
                {
                    columnName = memberExpression.Member.Name;
                }


                if (constantValue is string str)
                {
                    _whereConditions.Push($" [{_tableNameAliase}].[{columnName}] LIKE '%{str}%'");
                }
                else if (constantValue is IEnumerable<object> values)
                {
                    var valuesList = string.Join(",", values.Select(x => $"'{x}'"));
                    _whereConditions.Push($" [{_tableNameAliase}].[{columnName}] IN ({valuesList})");
                }
            }
                else if(node.Method.Name =="Select")
            {
                    string columnName = null;
                    var constantValue = GetConstantValue(node.Arguments.First());
        }

            else
                throw new NotSupportedException();


            return node;
        }
  
    private object GetConstantValue(Expression expression)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.Constant:
                return ((ConstantExpression)expression).Value;

            case ExpressionType.MemberAccess:
                var memberExpression = (MemberExpression)expression;
                var memberValue = GetConstantValue(memberExpression.Expression);
                if (memberValue == null)
                    throw new InvalidOperationException("Cannot evaluate member access expression.");
                var member = memberExpression.Member;
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        return ((FieldInfo)member).GetValue(memberValue);
                    case MemberTypes.Property:
                        return member;//((PropertyInfo)member).GetValue(memberValue);
                    default:
                        throw new NotSupportedException($"Member access of type '{member.MemberType}' is not supported.");
                }

            case ExpressionType.Call:
                var methodCallExpression = (MethodCallExpression)expression;
                var arguments = methodCallExpression.Arguments.Select(GetConstantValue).ToArray();
                var instance = methodCallExpression.Object != null ? GetConstantValue(methodCallExpression.Object) : null;

                // Check if it's a static method call or an instance method call
                if (methodCallExpression.Object != null)
                {
                    // Instance method call: Provide the correct instance when invoking the method
                    return methodCallExpression.Method.Invoke(instance, arguments);
                }
                else
                {
                    // Static method call: Use null as the instance when invoking the method
                    return methodCallExpression.Method.Invoke(null, arguments);
                }

            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
                var unaryExpression = (UnaryExpression)expression;
                return GetConstantValue(unaryExpression.Operand);

            case ExpressionType.Lambda:
                var lambdaExpression = (LambdaExpression)expression;
                return GetConstantValue(lambdaExpression.Body);

            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.Or:
            case ExpressionType.OrElse:
                var binaryExpression = (BinaryExpression)expression;
                var left = GetConstantValue(binaryExpression.Left);
                var right = GetConstantValue(binaryExpression.Right);
                return EvaluateBinaryExpression(binaryExpression.NodeType, left, right);
            case ExpressionType.Quote:
                {
                      unaryExpression = (UnaryExpression)expression;
                    return GetConstantValue(unaryExpression.Operand);
                }
            case ExpressionType.Parameter:
                {
                    var parameterExpression = (ParameterExpression)expression;

                      return parameterExpression;
                }

            // Add cases for other ExpressionTypes as needed...

            default:
                throw new NotSupportedException($"Expression of type '{expression.NodeType}' is not supported.");
        }
    }

    private object EvaluateBinaryExpression(ExpressionType binaryType, object left, object right)
    {
        
        switch (binaryType)
        {
            case ExpressionType.Equal:
                return Equals(left, right);
            case ExpressionType.NotEqual:
                return !Equals(left, right);
            case ExpressionType.GreaterThan:
                return Comparer.Default.Compare(left, right) > 0;
            case ExpressionType.GreaterThanOrEqual:
                return Comparer.Default.Compare(left, right) >= 0;
            case ExpressionType.LessThan:
                return Comparer.Default.Compare(left, right) < 0;
            case ExpressionType.LessThanOrEqual:
                return Comparer.Default.Compare(left, right) <= 0;
            case ExpressionType.And:
            case ExpressionType.AndAlso:
                return (bool)left && (bool)right;
            case ExpressionType.Or:
            case ExpressionType.OrElse:
                return (bool)left || (bool)right;
          
            default:
                throw new NotSupportedException($"Binary expression of type '{binaryType}' is not supported.");
        }
    }


    private static string GetMemberName(MemberExpression memberExpression)
        {
            if (memberExpression == null)
            {
                return null;
            }

            var instance = GetInstance(memberExpression.Expression);
            var memberInfo = memberExpression.Member;
            var memberValue = instance.GetType().GetProperty(memberInfo.Name)?.GetValue(instance);

            return memberValue?.ToString();
        }

        private static object GetInstance(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            if (expression is MemberExpression memberExpression)
            {
                var instance = GetInstance(memberExpression.Expression);
                var member = memberExpression.Member;

                if (member is FieldInfo field)
                {
                    return field.GetValue(instance);
                }
                else if (member is PropertyInfo property)
                {
                    return property.GetValue(instance);
                }
            }
            else if (expression is ConstantExpression constantExpression)
            {
                return constantExpression.Value;
            }


            

            return null;
        }

   

    }

public class QueryProvider : IQueryProvider
{
    private readonly IDbContext queryContext;

    public QueryProvider(IDbContext queryContext)
    {
        this.queryContext = queryContext;
    }

    public virtual IQueryable CreateQuery(Expression expression)
    {
        Type elementType = expression.Type;// TypeSystem.GetElementType();
        try
        {
            return
               (IQueryable)Activator.CreateInstance(typeof(Queryable<>).
                      MakeGenericType(elementType), new object[] { this, expression });
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }

    public virtual IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return new Queryable<T>(this, expression);
    }

    object IQueryProvider.Execute(Expression expression)
    {
        return queryContext.Execute(expression, false);
    }

    T IQueryProvider.Execute<T>(Expression expression)
    {
        return (T)queryContext.Execute(expression,
                   (typeof(T).Name == "IEnumerable`1"));
    }
}
 
public class Queryable<T> : IOrderedQueryable<T>
{
    public Queryable(IDbContext queryContext)
    {
        Initialize(new QueryProvider(queryContext), null);
    }

    public Queryable(IQueryProvider provider)
    {
        Initialize(provider, null);
    }

    internal Queryable(IQueryProvider provider, Expression expression)
    {
        Initialize(provider, expression);
    }

    private void Initialize(IQueryProvider provider, Expression expression)
    {
        if (provider == null)
            throw new ArgumentNullException("provider");
        if (expression != null && !typeof(IQueryable<T>).
               IsAssignableFrom(expression.Type))
            throw new ArgumentException(
                 String.Format("Not assignable from {0}", expression.Type), "expression");

        Provider = provider;
        Expression = expression ?? Expression.Constant(this);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
    }

   IEnumerator  IEnumerable.GetEnumerator()
    {
        return (Provider.Execute<System.Collections.IEnumerable>(Expression)).GetEnumerator();
    }

    public Type ElementType
    {
        get { return typeof(T); }
    }

    public Expression Expression { get; private set; }
    public IQueryProvider Provider { get; private set; }
}
