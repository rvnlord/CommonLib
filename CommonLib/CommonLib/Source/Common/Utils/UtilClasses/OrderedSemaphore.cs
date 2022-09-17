using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class OrderedSemaphore
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<TaskCompletionSource<bool>> _queue = new();

        public int CurrentCount => _semaphore.CurrentCount;

        public OrderedSemaphore(int initialCount)
        {
            _semaphore = new SemaphoreSlim(initialCount);
        }

        public OrderedSemaphore(int initialCount, int maxCount)
        {
            _semaphore = new SemaphoreSlim(initialCount, maxCount);
        }
        
        public void Wait() => WaitAsync().Sync();

        public Task WaitAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _queue.Enqueue(tcs);
            _semaphore.WaitAsync().ContinueWith(_ =>
            {
                if (_queue.TryDequeue(out var popped))
                    popped.SetResult(true);
            });
            return tcs.Task;
        }

        public async Task ReleaseAsync()
        {
            Release();
            await Task.CompletedTask;
        }

        public void Release() => _semaphore.Release();
    }
}
