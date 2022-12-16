using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class OrderedSemaphore : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<TaskCompletionSource<bool>> _queue = new();

        public int CurrentCount => _semaphore.CurrentCount;
        public bool IsDisposed { get; private set; }

        public OrderedSemaphore(int initialCount)
        {
            _semaphore = new SemaphoreSlim(initialCount);
        }

        public OrderedSemaphore(int initialCount, int maxCount)
        {
            _semaphore = new SemaphoreSlim(initialCount, maxCount);
        }

        public void Wait(TimeSpan? throwAfter = null) => WaitAsync(throwAfter).Wait();

        public Task WaitAsync(TimeSpan? throwAfter = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            _queue.Enqueue(tcs);
            
            var waitForSempahoreTask = _semaphore.WaitAsync();
            var timeoutTask = Task.Delay(throwAfter ?? TimeSpan.FromSeconds(30));
            var waitForSemaphoreOrTimeoutTask = new List<Task> { waitForSempahoreTask, timeoutTask };

            Task.WhenAny(waitForSemaphoreOrTimeoutTask).ContinueWith(task =>
            {
                if (IsDisposed)
                    return;

                if (waitForSempahoreTask.IsCompletedSuccessfully)
                {
                    if (_queue.TryDequeue(out var popped))
                        popped.SetResult(true);
                }
                else if (timeoutTask?.IsCompletedSuccessfully == true)
                {
                    throw new TimeoutException();
                }
            });

            return tcs.Task;
        }

        public async Task ReleaseAsync()
        {
            Release();
            await Task.CompletedTask;
        }

        public void Release() => _semaphore.Release();

        public void Dispose()
        {
            IsDisposed = true;
            _semaphore.Dispose();
        }
    }
}
