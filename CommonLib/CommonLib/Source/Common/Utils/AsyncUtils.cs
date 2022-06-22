using System;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Utils
{
    public static class AsyncUtils
    {
        public static void Sync(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            task.GetAwaiter().GetResult();
        }

        public static TResult Sync<TResult>(Task<TResult> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return task.GetAwaiter().GetResult();
        }
    }
}
