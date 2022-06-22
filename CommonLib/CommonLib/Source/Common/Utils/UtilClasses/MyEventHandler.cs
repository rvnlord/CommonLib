using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public delegate void MyEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e) where TEventArgs : EventArgs;
    public delegate Task MyAsyncEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e, CancellationToken token) where TEventArgs : EventArgs;
}
