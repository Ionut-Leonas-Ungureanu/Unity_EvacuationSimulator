using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Assets.Scripts.Utils
{
    class UnityDispatcher
    {
        private readonly ConcurrentQueue<Action> _tasks;

        public UnityDispatcher()
        {
            _tasks = new ConcurrentQueue<Action>();
        }

        public ThreadEvent Schedule(Action action)
        {
            var autoResetEvent = new ThreadEvent();
            
            _tasks.Enqueue(() =>
            {
                // Execute the given action
                action();
                // Notify that the action is finished
                autoResetEvent.Set();
            });

            return autoResetEvent;
        }

        public void Execute()
        {
            while (_tasks.Count != 0)
            {
                if (_tasks.TryDequeue(out var task))
                {
                    task?.Invoke();
                }
            }
        }
    }
}
