using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class OrderedSemaphore : IDisposable
    {
        private readonly int _maxCount;
        private readonly Queue<TaskCompletionSource<bool>> _queue;
        private readonly QueuedLock _queuedLock = new();

        public int CurrentCount { get; private set; }
        public bool IsDisposed { get; private set; }
        public string Description { get; set; }

        public OrderedSemaphore(int initialCount, int maxCount, string description = null)
        {
            if (initialCount < 0 || initialCount > maxCount)
                throw new ArgumentOutOfRangeException(nameof(initialCount));

            if (maxCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxCount));

            _maxCount = maxCount;
            CurrentCount = initialCount;
            _queue = new Queue<TaskCompletionSource<bool>>();
            Description = description;
        }

        public OrderedSemaphore(int initialCount) : this(initialCount, initialCount) { }

        public async Task WaitAsync(TimeSpan? timeoutAfter = null, Action action = null)
        {
            TaskCompletionSource<bool> tcs;
            try
            {
                _queuedLock.Enter();
                action?.Invoke();

                if (CurrentCount > 0)
                {
                    CurrentCount--;
                    return;
                }

                tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                _queue.Enqueue(tcs);
            }
            finally
            {
                _queuedLock.Exit();
            }

            if (await Task.WhenAny(tcs.Task, Task.Delay(timeoutAfter ?? TimeSpan.FromSeconds(300))) != tcs.Task)
                throw new TimeoutException();
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            try
            {
                _queuedLock.Enter();

                if (_queue.Count > 0)
                    toRelease = _queue.Dequeue();
                else if (CurrentCount < _maxCount)
                    CurrentCount++;
            }
            finally
            {
                _queuedLock.Exit();
            }

            toRelease?.SetResult(true);
        }

        public async Task ReleaseAsync()
        {
            Release();
            await Task.CompletedTask;
        }

        public async Task ReleaseSafelyAsync()
        {
            if (CurrentCount == 0)
                await ReleaseAsync();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        ~OrderedSemaphore()
        {
            Dispose();
        }
    }

    //public class OrderedSemaphore : IDisposable
    //{
    //    private readonly SemaphoreSlim _semaphore;
    //    private readonly ConcurrentQueue<TaskCompletionSource<bool>> _queue = new();

    //    public int CurrentCount => _semaphore.CurrentCount;
    //    public bool IsDisposed { get; private set; }
    //    public string Description { get; set; }

    //    public OrderedSemaphore(int initialCount)
    //    {
    //        _semaphore = new SemaphoreSlim(initialCount);
    //    }

    //    public OrderedSemaphore(int initialCount, int maxCount, string description = null)
    //    {
    //        _semaphore = new SemaphoreSlim(initialCount, maxCount);
    //        Description = description;
    //    }

    //    public void Wait(TimeSpan? throwAfter = null) => WaitAsync(throwAfter).Wait();

    //    public Task WaitAsync(TimeSpan? throwAfter = null, Action action = null)
    //    {
    //        var tcs = new TaskCompletionSource<bool>();
    //        _queue.Enqueue(tcs);
    //        action?.Invoke();

    //        var waitForSempahoreTask = _semaphore.WaitAsync();
    //        var timeoutTask = Task.Delay(throwAfter ?? TimeSpan.FromSeconds(30));
    //        var waitForSemaphoreOrTimeoutTask = new List<Task> { waitForSempahoreTask, timeoutTask };

    //        Task.WhenAny(waitForSemaphoreOrTimeoutTask).ContinueWith(task =>
    //        {
    //            if (IsDisposed)
    //                return;

    //            if (waitForSempahoreTask.IsCompletedSuccessfully)
    //            {
    //                if (_queue.TryDequeue(out var popped))
    //                    popped.SetResult(true);
    //            }
    //            else if (timeoutTask?.IsCompletedSuccessfully == true)
    //            {
    //                throw new TimeoutException();
    //            }
    //        });

    //        return tcs.Task;
    //    }

    //    public async Task ReleaseAsync()
    //    {
    //        Release();
    //        await Task.CompletedTask;
    //    }

    //    public void Release() => _semaphore.Release();

    //    public void Dispose()
    //    {
    //        IsDisposed = true;
    //        _semaphore.Dispose();
    //    }
    //}
}
