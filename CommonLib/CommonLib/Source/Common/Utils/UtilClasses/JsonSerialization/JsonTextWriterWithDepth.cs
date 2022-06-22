using System.IO;
using Newtonsoft.Json;

namespace CommonLib.Source.Common.Utils.UtilClasses.JsonSerialization
{
    public class JsonTextWriterWithDepth : JsonTextWriter
    {
        public JsonTextWriterWithDepth(TextWriter textWriter) : base(textWriter) { }

        public int CurrentDepth { get; private set; }

        public override void WriteStartObject()
        {
            CurrentDepth++;
            base.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            CurrentDepth--;
            base.WriteEndObject();
        }
    }
}
