using System.Threading.Tasks;
using CommonLib.Source.Common.Utils;
using CommonLib.Source.Common.Utils.UtilClasses;
using Microsoft.Extensions.Logging;

namespace CommonLib.Source.Common.Extensions
{
    public static class TaskExtensions
    {
        public static void Sync(this Task task) => AsyncUtils.Sync(task);
        public static TResult Sync<TResult>(this Task<TResult> task) => AsyncUtils.Sync(task);

        public static Task<T> LogAsync<T, TL>(this Task<T> t, ILogger<TL> logger, string message, LogLevel logLevel = LogLevel.Information)
        {
            logger.Log(logLevel, message);
            return t;
        }

        public static Task<T> LogAsync<T>(this Task<T> t, Logger logger, string message, NLog.LogLevel logLevel)
        {
            logger.Log(logLevel, message);
            return t;
        }
        
        public static Task<T> LogAsync<T>(this Task<T> t, Logger logger, string message) => t.LogAsync(logger, message, NLog.LogLevel.Info);
    }
}
