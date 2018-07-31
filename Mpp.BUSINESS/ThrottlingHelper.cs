using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mpp.BUSINESS
{
    public class ThrottlingHelper: IDisposable
    {
        //Holds time stamps for all started requests
        private readonly List<long> RequestsTx;
        private readonly ReaderWriterLockSlim Lock;

        private readonly int MaxLimit;
        private TimeSpan Interval;

        public ThrottlingHelper(int maxLimit, TimeSpan interval)
        {
            RequestsTx = new List<long>();
            MaxLimit = maxLimit;
            Interval = interval;
            Lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        public bool RequestAllowed
        {
            get
            {
                Lock.EnterReadLock();
                try
                {
                    var nowTx = DateTime.Now.Ticks;
                    return RequestsTx.Count(tx => nowTx - tx < Interval.Ticks) < MaxLimit;
                }
                finally
                {
                    Lock.ExitReadLock();
                }
            }
        }

        public void StartRequest()
        {
            Lock.EnterWriteLock();
            try
            {
                RequestsTx.Add(DateTime.Now.Ticks);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public void EndRequest()
        {
            Lock.EnterWriteLock();
            try
            {
                var nowTx = DateTime.Now.Ticks;
                RequestsTx.RemoveAll(tx => nowTx - tx >= Interval.Ticks);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            Lock.Dispose();
        }
    }
}
