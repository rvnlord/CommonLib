using System;
using CommonLib.Source.Common.Converters;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class FileSizeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FileSize);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteValue(((FileSize)value).SizeInBytes.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return new FileSize(reader.Value.ToLong());
        }
    }
}
