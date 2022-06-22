using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class ExceptionJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Exception);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var ex = (Exception)value;
            var exDict = new Dictionary<string, object>();

            var type = ex.GetType();
            exDict["ClassName"] = type.FullName;
            exDict["Message"] = ex.Message;
            exDict["Data"] = ex.Data;
            exDict["InnerException"] = ex.InnerException;
            exDict["HResult"] = ex.HResult;
            exDict["Source"] = ex.Source;

            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                if (!exDict.ContainsKey(p.Name))
                    exDict[p.Name] = p.GetValue(ex);

            serializer.Serialize(writer, exDict);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (JToken.Load(reader) is not JObject jException)
                return null;
            var innerEx = jException["InnerException"]?.To<Exception>();
            var message = jException["Message"]?.ToString();
            var className = jException["ClassName"]?.ToString();
            var ex = innerEx != null ? new Exception(message, innerEx) : new Exception(message);

            if (innerEx != null && className != null)
            {
                var type = TypeUtils.TypeUtils.GetTypeByName(className);
                var ctor = type.GetConstructor(new[] { typeof(string), typeof(Exception) });
                ex = ctor?.Invoke(new object[] { message, innerEx }) as Exception ?? ex;
            }
            else if (innerEx == null && className != null)
            {
                var type = TypeUtils.TypeUtils.GetTypeByName(className);
                var ctor = type.GetConstructor(new[] { typeof(string) });
                ex = ctor?.Invoke(new object[] { message }) as Exception ?? ex;
            }

            var data = jException["Data"].To<IDictionary>();
            if (data != null)
                ex.Data.ReplaceAll(data);
            ex.HResult = jException["HResult"].To<int>();
            ex.Source = jException["Source"]?.ToString();

            return ex;
        }
    }
}
