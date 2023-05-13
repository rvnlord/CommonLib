using System;
using CommonLib.Source.ViewModels.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class AuthenticationSchemeVMJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FileData);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var a = (AuthenticationSchemeVM) value;
            var jA = new JObject
            {
                [nameof(AuthenticationSchemeVM.Name)] = a.Name,
                [nameof(AuthenticationSchemeVM.DisplayName)] = a.DisplayName,
                [nameof(AuthenticationSchemeVM.HandlerType)] = a.HandlerType?.ToString() 
            };

            jA.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (JToken.Load(reader) is not JObject jA)
                return null;
            
            return new AuthenticationSchemeVM
            (
                jA[nameof(AuthenticationSchemeVM.Name)]?.ToString(),
                jA[nameof(AuthenticationSchemeVM.DisplayName)]?.ToString(),
                Type.GetType(jA[nameof(AuthenticationSchemeVM.HandlerType)]?.ToString() ?? "", false) ?? (Type) Type.Missing
            );
        }
    }
}
