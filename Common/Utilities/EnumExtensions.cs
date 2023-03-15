using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetEnumValues<T>(this T input) where T : struct
        {

            if (!typeof(T).IsEnum)
            { throw new NotSupportedException(); }
            return Enum.GetValues(input.GetType()).Cast<T>();

        }

        public static IEnumerable<T> GetEnumFlags<T>(this T input) where T : struct
        {

            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException();
            }

            foreach (var value in Enum.GetValues(input.GetType()))
            {
                if ((input as Enum).HasFlag(value as Enum))
                    yield return (T)value;
            }

        }

        public static string ToDisplay(this Enum value, string property = "Name")
        {
            Assert.NotNull(value, nameof(value));

            var attribute = value.GetType().GetField(value.ToString())
                .GetCustomAttributes(false)
                .OfType<DisplayAttribute>()
                .FirstOrDefault();

            if (attribute == null)
                return value.ToString();

            var propValue = attribute.GetType().GetProperty(property).GetValue(attribute, null);
            return propValue.ToString();
        }
    }
}
