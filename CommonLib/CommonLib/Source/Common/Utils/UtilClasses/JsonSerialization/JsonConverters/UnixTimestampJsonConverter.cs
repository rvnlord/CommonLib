﻿using System;
using CommonLib.Source.Common.Converters;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class UnixTimestampJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnixTimestamp);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteValue(((UnixTimestamp)value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return reader.Value?.ToDouble().ToUnixTimestamp();
        }
    }
}
