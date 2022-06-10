using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    class ThreadEvent
    {
        private readonly object _lock;
        private bool _isTriggered;

        public ThreadEvent()
        {
            _lock = new object();
        }

        public void WaitOne()
        {
            lock (_lock)
            {
                if (!_isTriggered)
                {
                    Monitor.Wait(_lock);
                }
                _isTriggered = false;
            }
        }

        public void Set()
        {
            lock (_lock)
            {
                _isTriggered = true;
                Monitor.PulseAll(_lock);
            }
        }

        public static void Pulse(object lockObject)
        {
            lock (lockObject)
            {
                Monitor.Pulse(lockObject);
            }
        }

        public static void Wait(object lockObject)
        {
            lock (lockObject)
            {
                Monitor.Wait(lockObject);
            }
        }
    }
}
