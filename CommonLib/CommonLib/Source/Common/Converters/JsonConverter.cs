using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization;
using CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CommonLib.Source.Common.Converters
{
    public static class JsonConverter
    {
        public static ITraceWriter SerializationLog { get; set; } = new MemoryTraceWriter();

        public static JsonSerializer JSerializer()
        {
            var jSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                TraceWriter = SerializationLog,
            };
            jSerializer.Converters.Add(new DecimalJsonConverter());
            jSerializer.Converters.Add(new LookupJsonConverter<string, string>());
            jSerializer.Converters.Add(new ExceptionJsonConverter());
            jSerializer.Converters.Add(new TypeJsonConverter());
            jSerializer.Converters.Add(new ExtendedTimeJsonConverter());
            jSerializer.Converters.Add(new FileSizeJsonConverter());
            jSerializer.Converters.Add(new FileDataJsonConverter());
            jSerializer.Converters.Add(new IconTypeJsonConverter());
            //jSerializer.Converters.Add(new ListJsonConverter());
            jSerializer.Converters.Add(new AuthenticationSchemeVMJsonConverter());
           
            return jSerializer;
        }

        public static JToken JsonDeserialize(this string str)
        {
            if (!str.IsJson())
                str = $"'{str}'";
            return JToken.Parse(str);
        }

        public static JToken JsonDeserializeOrNull(this string str)
        {
            try
            {
                return str.JsonDeserialize();
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public static string JsonSerialize(this object o)
        //{
        //    return o is JToken jt ? jt.ToString() : JsonConvert.SerializeObject(o, Formatting.Indented, JSerializer.Converters.ToArray());
        //}

        public static string JsonSerialize(this object o, int depth = 10)
        {
            using var strWriter = new StringWriter();
            var jsonWriter = new JsonTextWriterWithDepth(strWriter);
            var jSerializer = JSerializer();
            jSerializer.ContractResolver = new ConditionalJsonContractResolver(() => jsonWriter.CurrentDepth <= depth);
            jSerializer.Serialize(jsonWriter, o);
            jsonWriter.Close();
            return JToken.Parse(strWriter.ToString().Remove(@"\").TrimStart("\"{", 1).TrimEnd("}\"", 1)).RemoveEmptyDescendants().ToString(Formatting.Indented, jSerializer.Converters.ToArray());
        }

        public static JToken ToJToken(this object o, int depth = 10) => JToken.Parse(o.JsonSerialize(depth));
        //public static async Task<JToken> ToJTokenAsync(this Task<object> o, int depth = 10) => (await o).ToJToken(depth);
        public static async Task<JToken> ToJTokenAsync<T>(this Task<T> o, int depth = 10) => (await o).ToJToken(depth);

        public static T To<T>(this JToken jToken)
        {
            T o;
            try
            {
                // TODO: json conversion can't be typed because it will break in wasm as all types reflect to RuntimeType which will throw argument exception here in turn making i.e.: AuthenticationTypeScheme collection resolve to null breaking controls iteration in LoginBase
                o = !jToken.IsNullOrEmpty() ? jToken.ToObject<T>(JSerializer()) : (T)(object)null;
            }
            catch (JsonSerializationException)
            {
                return (T)(object)null;
            }

            if (o is null && typeof(T).IsIListType() && typeof(T).IsGenericType)
            {
                var elT = typeof(T).GetGenericArguments()[0];
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(elT);
                return (T)Activator.CreateInstance(constructedListType);
            }
            if (o is null && typeof(T).IsIDictionaryType() && typeof(T).IsGenericType)
            {
                var keyT = typeof(T).GetGenericArguments()[0];
                var valT = typeof(T).GetGenericArguments()[1];
                var dictType = typeof(Dictionary<,>);
                var constructedDictType = dictType.MakeGenericType(keyT, valT);
                return (T)Activator.CreateInstance(constructedDictType);
            }

            return o;
        }

        public static JObject ToJObject(this JToken jToken)
        {
            return (JObject)jToken;
        }

        public static JArray ToJArray(this JToken jToken)
        {
            return (JArray)jToken;
        }

        public static string ToStringN(this JToken jToken)
        {
            return jToken.IsNullOrEmpty() ? null : jToken.ToString();
        }

        public static T Parse<T>(this JToken jt, Func<JToken, T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            return parser(jt);
        }

        public static async Task<T> Parse<T>(this Task<JToken> taskStr, Func<JToken, T> parser)
        {
            if (taskStr == null)
                throw new ArgumentNullException(nameof(taskStr));

            return (await taskStr.ConfigureAwait(false)).Parse(parser);
        }
    }
}
