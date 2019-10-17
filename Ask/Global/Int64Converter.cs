using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using Newtonsoft.Json;

namespace WorkV3.Golbal
{    
    public class Int64Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            long v = value is ulong ? (long)(ulong)value : (long)value;
            writer.WriteValue(v.ToString());
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            string val = reader.Value as string;
            if (string.IsNullOrWhiteSpace(val))
                return 0;

            long v = long.Parse(val);
            return typeof(ulong) == objectType ? (object)(ulong)v : v;
        }
        public override bool CanConvert(Type objectType) {
            switch (objectType.FullName) {
                case "System.Int64":
                case "System.UInt64":
                    return true;
                default:
                    return false;
            }
        }
    }
}