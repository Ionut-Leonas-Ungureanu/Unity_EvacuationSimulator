using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Bot.Observers;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using System.Threading;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class ObserveBots : SimulatorState
    {
        private readonly IBotObserver _botObserver;
        private readonly object _lock;
        private uint _inactiveBotsCounter;

        public ObserveBots(SimulatorContext context) : base(context)
        {
            _lock = new object();
            _botObserver = new CustomizedBotObserver((botController) =>
            {
                lock (_lock)
                {
                    if (SimulationConfigurator.Instance.MiscellaneousSettings.Train && botController.Identifier == 0)
                    {
                        // Kill all remaining bots
                        foreach (var bot in _context.Simulator.BotsManager.Bots)
                        {
                            if (bot.IsActive)
                            {
                                bot.KillBot();
                            }
                        }
                    }

                    _inactiveBotsCounter++;

                    if (_inactiveBotsCounter == _context.Simulator.BotsManager.Bots.Length)
                    {
                        Monitor.Pulse(_lock);
                    }
                }
            });
        }

        protected override void HandleState()
        {
            _inactiveBotsCounter = 0;

            // Start bots
            foreach (var bot in _context.Simulator.BotsManager.Bots)
            {
                // Subscribe to bot
                bot.Subscribe(_botObserver);
                bot.HasToEscape = true;
            }

            lock (_lock)
            {
                Monitor.Wait(_lock);
            }

            // Unsubscribe from bots
            foreach (var bot in _context.Simulator.BotsManager.Bots)
            {
                bot.UnSubscribe(_botObserver);
            }
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.DISPLAY_STATS);
        }
    }
}
