using System.Threading;

namespace Assets.Scripts.Utils.Timer
{
    class ReactionTimer
    {
        #region Properties

        private System.Timers.Timer _timer;
        private bool _triggered;
        private object _lock; 

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public double Interval 
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        #endregion

        public ReactionTimer()
        {
            _lock = new object();

            _timer = new System.Timers.Timer
            {
                AutoReset = false,
            };
            _timer.Elapsed += ReactionTimeTimer_Elapsed;
        }

        ~ReactionTimer()
        {
            _timer.Dispose();
        }

        #region Methods

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            _triggered = false;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Blocks current thread until the time elapses if it is not already
        /// </summary>
        public void Wait()
        {
            lock (_lock)
            {
                if (!_triggered)
                {
                    Monitor.Wait(_lock);
                    _triggered = false;
                }
            }
        }

        private void ReactionTimeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lock)
            {
                _triggered = true;
                Monitor.Pulse(_lock);
            }
        }

        #endregion
    }
}
