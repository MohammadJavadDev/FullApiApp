using System.Data.SqlClient;
using Common.Utilities;

namespace MJ.EntityFramwork.TableGenerate
{
    public static class CreateClass
    {
        public  static void CreateClassFromTable( string tableNameCreate , string tableSchemaCreate , string path, string stringConnetion)
        {
            tableNameCreate = tableNameCreate.CapitalizeFirst();
            tableSchemaCreate = tableSchemaCreate.ToUpper();
  
            SqlConnection connection = new SqlConnection(stringConnetion);
            connection.Open();

            // Retrieve information about the columns in the table
            SqlCommand command = new SqlCommand($@"
                    SELECT COLUMN_NAME, 
                    DATA_TYPE,
                    CHARACTER_MAXIMUM_LENGTH,
                    NUMERIC_PRECISION,
                    DATETIME_PRECISION,
                    IS_NULLABLE
                 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableNameCreate}'  AND TABLE_SCHEMA = '{tableSchemaCreate}'", connection);
          
            SqlDataReader reader = command.ExecuteReader();

            // Create a list of properties based on the columns in the table
            List<string> properties = new List<string>();
            while (reader.Read())
            {
                string columnName = reader.GetString(0);
                string dataType = reader.GetString(1);
                int? length = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
                byte? precision = reader.IsDBNull(3) ? null : (byte?)reader.GetByte(3);
                 
                int? dateTimePrecision = reader.IsDBNull(4) ? null : reader.GetValue(4)?.ToString()?.ToInt();
               
                string? isNULLABLE =    reader.GetString(5) == "YES" ? "?": "";

                string propertyType;
                switch (dataType)
                {
                    case "bigint":
                        propertyType = "long";
                        break;
                    case "binary":
                    case "image":
                    case "varbinary":
                        propertyType = "byte[]";
                        break;
                    case "bit":
                        propertyType = "bool";
                        break;
                    case "char":
                        propertyType = $"string";
                        break;
                    case "date":
                    case "datetime":
                    case "datetime2":
                    case "smalldatetime":
                        propertyType = "DateTime";
                        break;
                    case "decimal":
                    case "money":
                    case "numeric":
                    case "smallmoney":
                        propertyType = $"decimal";
                        break;
                    case "float":
                        propertyType = "double";
                        break;
                    case "int":
                        propertyType = "int";
                        break;
                    case "nchar":
                        propertyType = $"string";
                        break;
                    case "ntext":
                    case "nvarchar":
                    case "varchar":
                        propertyType = $"string";
                        break;
                    case "real":
                        propertyType = "float";
                        break;
                    case "smallint":
                        propertyType = "short";
                        break;
                    case "text":
                        propertyType = $"string";
                        break;
                    case "time":
                        propertyType = "TimeSpan";
                        break;
                    case "tinyint":
                        propertyType = "byte";
                        break;
                    case "uniqueidentifier":
                        propertyType = "Guid";
                        break;
                    case "xml":
                        propertyType = "string";
                        break;
                    default:
                        throw new Exception("Unknown data type: " + dataType);
                }

                var strProp = "";

                if (length.HasValue && length > 0)
                    strProp += $"[MaxLength({length})] \n";


                strProp += $"public {propertyType} {isNULLABLE} {columnName} {{ get; set; }}";

                    properties.Add(strProp);
            }
            reader.Close();
            command = new SqlCommand(@$"
       				 SELECT
                f.name AS ForeignKeyName,
                OBJECT_NAME(f.parent_object_id) AS TableName,
                COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,
                OBJECT_NAME(f.referenced_object_id) AS ReferencedTableName,
                COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColumnName,
			OBJECT_SCHEMA_NAME(f.referenced_object_id)  AS ReferencedTableSchema
            FROM
                sys.foreign_keys AS f
                INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id

   WHERE
                OBJECT_NAME(f.parent_object_id) ='{tableNameCreate}' AND  OBJECT_SCHEMA_NAME(f.object_id) = '{tableSchemaCreate}'
        ", connection);

            reader = command.ExecuteReader();

            // Create a list of foreign keys based on the foreign keys in the table
            List<string> foreignKeys = new List<string>();
            while (reader.Read())
            {
                string foreignKeyName = reader.GetString(0);
                string tableName = reader.GetString(1);
                string columnName = reader.GetString(2).CapitalizeFirst();
                string referencedTableName = reader.GetString(3).CapitalizeFirst();
                string referencedColumnName = reader.GetString(4).CapitalizeFirst();
                string referencedColumnSchema = reader.GetString(5).ToUpper();

                string propertyName = $"{referencedTableName}{referencedColumnName}";
                foreignKeys.Add($"public {referencedColumnSchema}{referencedTableName} {propertyName.Replace("Id","")} {{ get; set; }}");
            }
            reader.Close();

            // Retrieve information about the primary key in the table
            command = new SqlCommand(@$"
            SELECT
                tc.CONSTRAINT_NAME AS PrimaryKeyName,
                ccu.COLUMN_NAME AS ColumnName
            FROM
                INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
            WHERE
                tc.TABLE_NAME = '{tableNameCreate}' AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND ccu.TABLE_SCHEMA = '{tableSchemaCreate}'", connection);
            reader = command.ExecuteReader();

            // Create a list of properties for the primary key
            List<string> primaryKeyProperties = new List<string>();
            while (reader.Read())
            {
                string primaryKeyName = reader.GetString(0);
                string columnName = reader.GetString(1);

                primaryKeyProperties.Add($"[Key]\npublic {properties.Find(p => p.Contains(columnName)).Substring(7)}");
                properties.Remove(properties.Find(p => p.Contains(columnName)));
            }
            reader.Close();

            // Combine all the properties and foreign keys into a single class definition
            string className = tableNameCreate;
            string classCode = $"using System.ComponentModel.DataAnnotations;\nusing System.ComponentModel.DataAnnotations.Schema;\nnamespace Entities.Generated {{ \n[Table(\"{tableNameCreate}\",Schema =\"{tableSchemaCreate}\")] \n public class {tableSchemaCreate}{className} {{\n{string.Join("\n", primaryKeyProperties)}\n{string.Join("\n", properties)}\n{string.Join("\n", foreignKeys)}\n}} \n}}";

            // Write the class code to a file
            string filePath = $"{path}/{tableSchemaCreate}/{tableSchemaCreate}{className}.cs";
            string dir = $"{path}/{tableSchemaCreate}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(filePath, classCode);
 
            reader.Close();
            connection.Close();
        }

        public static void CreateClassFromDB(string connectionString , string path)
        {
            var listTables = new List<TableInformation>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

              
                SqlCommand command = new SqlCommand(@"
                    SELECT 
                            TABLE_NAME,
                            TABLE_SCHEMA
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE = 'BASE TABLE'", connection);

                SqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("Tables in the database:");
                while (reader.Read())
                {
                  
                    listTables.Add(
                        new TableInformation(reader.GetString(0),reader.GetString(1))
                        );
                }

                reader.Close();
            }

            listTables.ForEach(c => CreateClassFromTable(c.Name, c.Schema, path , connectionString));
        }
        
    }

    public class TableInformation
    {
        public string?  Name { get; set; }
        public string? Schema { get; set; }
       public  TableInformation(string name, string schema)
        {
            Name = name;    
            Schema = schema;
        }
    }

}
