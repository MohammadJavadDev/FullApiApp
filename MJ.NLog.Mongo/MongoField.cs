using NLog.Config;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJ.NLog.Mongo
{
    [NLogConfigurationItem]
    [ThreadAgnostic]
    public sealed class MongoField
    {
 
        public MongoField()
            : this(null, null, "String")
        {
        }

 
        public MongoField(string name, Layout layout)
            : this(name, layout, "String")
        {
        }
 
        public MongoField(string name, Layout layout, string bsonType)
        {
            Name = name;
            Layout = layout;
            BsonType = bsonType ?? "String";
        }

 
        [RequiredParameter]
        public string Name { get; set; }

 
        [RequiredParameter]
        public Layout Layout { get; set; }

 
        [DefaultValue("String")]
        public string BsonType
        {
            get => _bsonType;
            set
            {
                _bsonType = value;
                BsonTypeCode = ConvertToTypeCode(value?.Trim() ?? string.Empty);
            }
        }
        private string _bsonType;

        internal TypeCode BsonTypeCode { get; private set; } = TypeCode.String;

        private TypeCode ConvertToTypeCode(string bsonType)
        {
            if (string.IsNullOrEmpty(bsonType) || string.Equals(bsonType, "String", StringComparison.OrdinalIgnoreCase))
                return TypeCode.String;

            if (string.Equals(bsonType, "Boolean", StringComparison.OrdinalIgnoreCase))
                return TypeCode.Boolean;

            if (string.Equals(bsonType, "DateTime", StringComparison.OrdinalIgnoreCase))
                return TypeCode.DateTime;

            if (string.Equals(bsonType, "Double", StringComparison.OrdinalIgnoreCase))
                return TypeCode.Double;

            if (string.Equals(bsonType, "Int32", StringComparison.OrdinalIgnoreCase))
                return TypeCode.Int32;

            if (string.Equals(bsonType, "Int64", StringComparison.OrdinalIgnoreCase))
                return TypeCode.Int64;

            if (string.Equals(bsonType, "Object", StringComparison.OrdinalIgnoreCase))
                return TypeCode.Object;

            return TypeCode.String;
        }
    }
}
