using System;
using System.Reflection;
using WebSocketSharp;
using Logger = NLog.Logger;

namespace CommonLib.Source.Common.Extensions
{
    public static class WebSocketSharpLoggerExtensions
    {
        public static void Disable(this Logger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var field = logger.GetType().GetField("_output", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(logger, new Action<LogData, string>((d, s) => { }));
        }
    }
}
