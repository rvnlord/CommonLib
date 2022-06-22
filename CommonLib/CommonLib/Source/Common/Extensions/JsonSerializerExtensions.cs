using System;
using Newtonsoft.Json;

namespace CommonLib.Source.Common.Extensions
{
    public static class JsonSerializerExtensions
    {
        public static JsonSerializerSettings GetSettings(this JsonSerializer serializer)
        {
            if (serializer == null)
                throw new NullReferenceException(nameof(serializer));

            var settings = new JsonSerializerSettings
            {
                Context = serializer.Context,
                Culture = serializer.Culture,
                ContractResolver = serializer.ContractResolver,
                ConstructorHandling = serializer.ConstructorHandling,
                CheckAdditionalContent = serializer.CheckAdditionalContent,
                DateFormatHandling = serializer.DateFormatHandling,
                DateFormatString = serializer.DateFormatString,
                DateParseHandling = serializer.DateParseHandling,
                DateTimeZoneHandling = serializer.DateTimeZoneHandling,
                DefaultValueHandling = serializer.DefaultValueHandling,
                EqualityComparer = serializer.EqualityComparer,
                FloatFormatHandling = serializer.FloatFormatHandling,
                Formatting = serializer.Formatting,
                FloatParseHandling = serializer.FloatParseHandling,
                MaxDepth = serializer.MaxDepth,
                MetadataPropertyHandling = serializer.MetadataPropertyHandling,
                MissingMemberHandling = serializer.MissingMemberHandling,
                NullValueHandling = serializer.NullValueHandling,
                ObjectCreationHandling = serializer.ObjectCreationHandling,
                PreserveReferencesHandling = serializer.PreserveReferencesHandling,
                ReferenceResolverProvider = () => serializer.ReferenceResolver,
                ReferenceLoopHandling = serializer.ReferenceLoopHandling,
                StringEscapeHandling = serializer.StringEscapeHandling,
                TraceWriter = serializer.TraceWriter,
                TypeNameHandling = serializer.TypeNameHandling,
                SerializationBinder = serializer.SerializationBinder,
                TypeNameAssemblyFormatHandling = serializer.TypeNameAssemblyFormatHandling
            };
            foreach (var converter in serializer.Converters)
                settings.Converters.Add(converter);

            return settings;
        }

        public static JsonSerializerSettings SetSettings(this JsonSerializerSettings oldSettings, JsonSerializerSettings newSettings)
        {
            if (oldSettings == null)
                throw new NullReferenceException(nameof(oldSettings));
            if (newSettings == null)
                throw new NullReferenceException(nameof(newSettings));

            oldSettings.Context = newSettings.Context;
            oldSettings.Culture = newSettings.Culture;
            oldSettings.ContractResolver = newSettings.ContractResolver;
            oldSettings.ConstructorHandling = newSettings.ConstructorHandling;
            oldSettings.CheckAdditionalContent = newSettings.CheckAdditionalContent;
            oldSettings.DateFormatHandling = newSettings.DateFormatHandling;
            oldSettings.DateFormatString = newSettings.DateFormatString;
            oldSettings.DateParseHandling = newSettings.DateParseHandling;
            oldSettings.DateTimeZoneHandling = newSettings.DateTimeZoneHandling;
            oldSettings.DefaultValueHandling = newSettings.DefaultValueHandling;
            oldSettings.EqualityComparer = newSettings.EqualityComparer;
            oldSettings.FloatFormatHandling = newSettings.FloatFormatHandling;
            oldSettings.Formatting = newSettings.Formatting;
            oldSettings.FloatParseHandling = newSettings.FloatParseHandling;
            oldSettings.MaxDepth = newSettings.MaxDepth;
            oldSettings.MetadataPropertyHandling = newSettings.MetadataPropertyHandling;
            oldSettings.MissingMemberHandling = newSettings.MissingMemberHandling;
            oldSettings.NullValueHandling = newSettings.NullValueHandling;
            oldSettings.ObjectCreationHandling = newSettings.ObjectCreationHandling;
            oldSettings.PreserveReferencesHandling = newSettings.PreserveReferencesHandling;
            oldSettings.ReferenceResolverProvider = newSettings.ReferenceResolverProvider;
            oldSettings.ReferenceLoopHandling = newSettings.ReferenceLoopHandling;
            oldSettings.StringEscapeHandling = newSettings.StringEscapeHandling;
            oldSettings.TraceWriter = newSettings.TraceWriter;
            oldSettings.TypeNameHandling = newSettings.TypeNameHandling;
            oldSettings.SerializationBinder = newSettings.SerializationBinder;
            oldSettings.TypeNameAssemblyFormatHandling = newSettings.TypeNameAssemblyFormatHandling;

            foreach (var converter in newSettings.Converters)
                oldSettings.Converters.Add(converter);

            return oldSettings;
        }
    }
}
