using System;
using System.Threading.Tasks;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class TaskUtils
    {
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task WaitWhileAsync(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) 
                    await Task.Delay(frequency).ConfigureAwait(false);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)).ConfigureAwait(false))
                throw new TimeoutException();
        }

        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async Task WaitUntilAsync(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) 
                    await Task.Delay(frequency).ConfigureAwait(false);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)).ConfigureAwait(false))
                throw new TimeoutException();
        }

        public static Task WaitUntilAsync(Func<bool> condition, int frequency, TimeSpan timeout) => WaitUntilAsync(condition, frequency, timeout.TotalMilliseconds.ToInt());
    }
}
