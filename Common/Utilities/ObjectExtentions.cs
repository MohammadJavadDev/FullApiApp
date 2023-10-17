using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
 

namespace Common.Utilities
{
    public static class ObjectExtentions
    {
        public static string JsonSerialize(this object obj)
        {
            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            return JsonSerializer.Serialize(obj , jso);
        }

        public static T? JsonDeserialize<T>(this string obj)
        {
            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            jso.PropertyNameCaseInsensitive = true;
            jso.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            return JsonSerializer.Deserialize<T>(obj, jso);
        }

    }
}
