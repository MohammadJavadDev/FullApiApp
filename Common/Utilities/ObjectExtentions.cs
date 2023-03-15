using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class ObjectExtentions
    {
        public static string JsonSerialize(this object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T? JsonDeserialize<T>(this string obj)
        {
            return JsonSerializer.Deserialize<T>(obj);
        }

    }
}
