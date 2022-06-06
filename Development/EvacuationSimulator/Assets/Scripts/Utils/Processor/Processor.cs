using System;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils.Processor
{
    class Processor
    {
        #region Properties

        private Thread _thread;
        private Action _task;
        private Action _stopTask;
        private bool _run;

        #endregion

        public Processor(Action task, Action stopTask = null)
        {
            _task = task;
            _stopTask = stopTask;
        }

        #region Methods

        public void Start()
        {
            _run = true;
            _thread = new Thread(RunTask)
            {
                IsBackground = true,
                Priority = System.Threading.ThreadPriority.Highest
            };
            _thread.Start();
        }

        public void Stop()
        {
            _run = false;
            _stopTask?.Invoke();
        }

        private void RunTask()
        {
            while (_run)
            {
                try
                {
                    _task?.Invoke();
                }
                catch(Exception ex)
                {
                    Debug.LogWarning($"{ex.Message} + {ex.StackTrace}");
                }
            }
        }

        #endregion
    }
}
