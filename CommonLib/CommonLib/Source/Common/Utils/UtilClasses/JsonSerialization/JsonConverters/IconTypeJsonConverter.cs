using System;
using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class IconTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IconType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var icon = (IconType) value;


            var jIcon = new JObject
            {
                [nameof(IconType.SetName)] = icon.SetName,
                [nameof(IconType.IconName)] = icon.IconName
            };

            jIcon.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (JToken.Load(reader) is not JObject jIcon)
                return null;

            return new IconType(jIcon[nameof(IconType.SetName)]?.ToString(), jIcon[nameof(IconType.IconName)]?.ToString());
        }
    }
}
