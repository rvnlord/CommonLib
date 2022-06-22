using System.Threading;
using CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization.JsonConverters;
using Newtonsoft.Json;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization
{
    public static class CustomJsonSerializer
    {
        private static JsonSerializer _s; // or if not synced then we can try to reinitialize it as a new object like in Blazor Tutorial

        public static JsonSerializer JSerializer => _s ??= Get();
        public static SemaphoreSlim SyncSerializer { get; } = new(1, 1);

        private static JsonSerializer Get() => Converters.JsonConverter.JSerializer();
    }
}
