using System;
using CommonLib.Source.Common.Converters;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class TypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Type);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteValue(((Type)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return Type.GetType(reader.Value?.ToString() ?? "", false) ?? Type.Missing; // do not throw if we are deserializing types that are not present in the project (like when we are getting Authentication Schemes for display from Server but CLient is wasm and doesn't support them)
        }
    }
}
