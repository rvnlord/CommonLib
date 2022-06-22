using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class LookupJsonConverter<TKey, TValue> : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (!objectType.IsValueType && objectType.IsGenericType)
                return objectType.GetGenericTypeDefinition().In(typeof(ILookup<,>), typeof(Lookup<,>));

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var lookup = (ILookup<TKey, TValue>)value;
            var kvps = lookup.SelectMany(kvp => kvp.ToList().Select(v => new KeyValuePair<TKey, TValue>(kvp.Key, v)));

            serializer.Serialize(writer, kvps);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (JToken.Load(reader) is not JArray jLookup)
                return null;
            var kvps = jLookup.Select(jKvp => new KeyValuePair<TKey, TValue>(
                (TKey)Convert.ChangeType(jKvp["Key"], typeof(TKey)),
                (TValue)Convert.ChangeType(jKvp["Value"], typeof(TValue)))).ToList();

            return kvps.ToLookup();
        }
    }
}
