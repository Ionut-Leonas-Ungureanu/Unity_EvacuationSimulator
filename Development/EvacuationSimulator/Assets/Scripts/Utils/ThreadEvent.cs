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
            //Debug.Log(">> Ask for wait lock");
            lock (_lock)
            {
                if (!_isTriggered)
                {
                    //Debug.Log(">> Start waiting");
                    Monitor.Wait(_lock);
                }
                _isTriggered = false;
            }
            //Debug.Log(">> Out of waiting and lock");
        }

        public void Set()
        {
            //Debug.Log(">> Ask for pulse lock");
            lock (_lock)
            {
                _isTriggered = true;
                Monitor.PulseAll(_lock);
                //Debug.Log(">> Pulse ALL");
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
