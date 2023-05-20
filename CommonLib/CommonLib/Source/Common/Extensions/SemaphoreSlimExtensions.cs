using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Extensions
{
    public static class SemaphoreSlimExtensions
    {
        public static async Task ReleaseSafelyAsync(this SemaphoreSlim semaphore)
        {
            if (semaphore.CurrentCount == 0)
                await semaphore.ReleaseAsync();
        }

        public static async Task ReleaseAsync(this SemaphoreSlim semaphore)
        {
            await Task.FromResult(semaphore.Release());
        }
    }
}
