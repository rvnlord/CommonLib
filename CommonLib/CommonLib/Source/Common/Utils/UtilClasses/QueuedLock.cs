using System.Threading;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public sealed class QueuedLock
    {
        private readonly object _innerLock;
        private volatile int _ticketsCount;
        private volatile int _ticketToRide = 1;
        private readonly ThreadLocal<int> _reEnter = new();

        public QueuedLock()
        {
            _innerLock = new object();
        }

        public void Enter()
        {
            _reEnter.Value++;
            if (_reEnter.Value > 1)
                return;
            var myTicket = Interlocked.Increment(ref _ticketsCount);
            Monitor.Enter(_innerLock);
            while (true)
            {

                if (myTicket == _ticketToRide)
                    return;

                Monitor.Wait(_innerLock);
            }
        }

        public void Exit()
        {
            if (_reEnter.Value > 0)
                _reEnter.Value--;
            if (_reEnter.Value > 0)
                return;
            Interlocked.Increment(ref _ticketToRide);
            Monitor.PulseAll(_innerLock);
            Monitor.Exit(_innerLock);
        }
    }
}
