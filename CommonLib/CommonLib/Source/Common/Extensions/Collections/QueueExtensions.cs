using System.Collections.Generic;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class QueueExtensions
    {
        public static T DequeueOrDefault<T>(this Queue<T> queue) => queue.TryDequeue(out var n) ? n : default;
        public static T DequeueOrNull<T>(this Queue<T> queue) where T : class => queue.TryDequeue(out var n) ? n : null;
        public static T PeekOrDefault<T>(this Queue<T> queue) => queue.TryPeek(out var n) ? n : default;
        public static T PeekOrNull<T>(this Queue<T> queue) where T : class => queue.TryPeek(out var n) ? n : null;
    }
}
