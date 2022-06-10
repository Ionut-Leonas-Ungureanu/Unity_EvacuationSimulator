using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils.Experience
{
    class ExperienceBuffer<T>
    {
        private ConcurrentQueue<T> _buffer;
        private readonly object _lock;

        public ExperienceBuffer()
        {
            _buffer = new ConcurrentQueue<T>();
            _lock = new object();
        }

        public void Enqueue(T item)
        {
            _buffer.Enqueue(item);
            Notify();
        }

        public bool TryDequeue(out T item)
        {
            return _buffer.TryDequeue(out item);
        }

        /// <summary>
        /// Blocks current thread until a notify event is received:
        /// - an item is inserted in buffer
        /// </summary>
        public void Wait()
        {
            lock (_lock)
            {
                Monitor.Wait(_lock);
            }
        }

        /// <summary>
        /// Blocks current thread until a notify event is received:
        /// - an item is inserted in buffer
        /// </summary>
        /// <param name="milliseconds"></param>
        public void Wait(int milliseconds)
        {
            lock (_lock)
            {
                Monitor.Wait(_lock, milliseconds);
            }
        }

        private void Notify()
        {
            lock (_lock)
            {
                Monitor.Pulse(_lock);
            }
        }
    }
}
