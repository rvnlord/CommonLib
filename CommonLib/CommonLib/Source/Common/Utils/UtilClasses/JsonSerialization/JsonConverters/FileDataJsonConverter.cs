using System;
using System.Linq;
using CommonLib.Source.Common.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters
{
    public class FileDataJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FileData);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var fd = (FileData) value;
            var jFd = new JObject
            {
                [nameof(FileData.Name)] = fd.Name,
                [nameof(FileData.Extension)] = fd.Extension,
                [nameof(FileData.TotalSizeInBytes)] = fd.TotalSizeInBytes?.ToString(),
                [nameof(FileData.DirectoryPath)] = fd.DirectoryPath,
                [nameof(FileData.Position)] = fd.Position.ToString(),
                [nameof(FileData.IsSelected)] = fd.IsSelected.ToString().ToLowerInvariant(),
                [nameof(FileData.Status)] = fd.Status.EnumToString(),
                [nameof(FileData.Data)] = fd.Data?.ToHexString()
            };

            jFd.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (JToken.Load(reader) is not JObject jFd)
                return null;
            
            return new FileData
            {
                Name = jFd[nameof(FileData.Name)]?.ToString(),
                Extension = jFd[nameof(FileData.Extension)]?.ToString(),
                TotalSizeInBytes = jFd[nameof(FileData.TotalSizeInBytes)]?.ToLong(),
                DirectoryPath =  jFd[nameof(FileData.DirectoryPath)]?.ToString(),
                Position = jFd[nameof(FileData.Position)].ToLong(),
                IsSelected = jFd[nameof(FileData.IsSelected)].ToBool(),
                Status = jFd[nameof(FileData.Status)].ToEnum<UploadStatus>(),
                Data = jFd[nameof(FileData.Data)]?.ToString().HexToByteList()
            };
        }
    }
}
