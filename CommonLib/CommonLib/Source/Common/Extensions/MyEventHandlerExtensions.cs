using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Source.Common.Utils.UtilClasses;
using MoreLinq.Extensions;

namespace CommonLib.Source.Common.Extensions
{
    public static class MyEventHandlerExtensions
    {
        public static void Invoke<TSender, TEventArgs>(this MyEventHandler<TSender, TEventArgs> handler, TSender sender, TEventArgs args) where TEventArgs : EventArgs
        {
            var delegates = handler?.GetInvocationList();
            if (delegates?.Any() == true)
                delegates.Cast<MyEventHandler<TSender, TEventArgs>>().ForEach(e => e.Invoke(sender, args));
        }

        public static async Task InvokeAsync<TSender, TEventArgs>(this MyAsyncEventHandler<TSender, TEventArgs> handler, TSender sender, TEventArgs args, CancellationToken cancellationToken) where TEventArgs : EventArgs
        {
            var delegates = handler?.GetInvocationList();
            if (delegates?.Any() == true) 
            {
                var tasks = delegates.Cast<MyAsyncEventHandler<TSender, TEventArgs>>().Select(e => e.Invoke(sender, args, cancellationToken));
                await Task.WhenAll(tasks);
            }
        }

        public static async Task InvokeAsync<TSender, TEventArgs>(this MyAsyncEventHandler<TSender, TEventArgs> handler, TSender sender, TEventArgs args) where TEventArgs : EventArgs
        {
            await handler.InvokeAsync(sender, args, CancellationToken.None);
        }
    }
}
