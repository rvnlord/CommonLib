using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    // CURRENTLY NOT USED, not registered in `CommonLib.Source.Common.Converters.JsonConverter`, it wouldn't override the default properly like this anyway
    public class ListJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var list = (List<object>)value;

            writer.WriteStartArray();

            if (value is not null)
                foreach (var item in list)
                    serializer.Serialize(writer, item);

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (JToken.Load(reader) is not JArray)
                return null;

            var list = new List<object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                    return list;

                var item = serializer.Deserialize(reader, objectType);
                list.Add(item);
            }

            throw new JsonSerializationException("Unexpected end when reading List<T>.");
        }
    }
}
