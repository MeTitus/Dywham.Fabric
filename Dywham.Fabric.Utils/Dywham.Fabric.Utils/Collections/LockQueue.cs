using System.Threading;

namespace Dywham.Fabric.Utils.Collections
{
    public sealed class LockQueue
    {
        private readonly object _innerLock = new();
        private volatile int _ticketsCount;
        private volatile int _ticketToRide = 1;
        private readonly ThreadLocal<int> _reenter = new();


        public void Enter()
        {
            _reenter.Value++;

            if (_reenter.Value > 1) return;

            var myTicket = Interlocked.Increment(ref _ticketsCount);

            Monitor.Enter(_innerLock);

            while (true)
            {
                if (myTicket == _ticketToRide)
                {
                    return;
                }

                Monitor.Wait(_innerLock);
            }
        }

        public void Exit()
        {
            if (_reenter.Value > 0) _reenter.Value--;

            if (_reenter.Value > 0) return;

            Interlocked.Increment(ref _ticketToRide);

            Monitor.PulseAll(_innerLock);

            Monitor.Exit(_innerLock);
        }
    }
}