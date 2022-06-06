using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using Assets.Scripts.Utils;
using System.Threading;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class GenerateBots : SimulatorState
    {
        private readonly object _lock;

        public GenerateBots(SimulatorContext context) : base(context)
        {
            _lock = new object();
            _context.Simulator.BotsManager.OnInitialized = () =>
            {
                ThreadEvent.Pulse(_lock);
            };
        }

        protected override void HandleState()
        {
            Dialogs.OnDialogClose = () =>
            {
                ThreadEvent.Pulse(_lock);
            };

            // Start generating bots
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                _context.Simulator.StartCoroutine(Dialogs.ShowBotsDialog());
                _context.Simulator.StartCoroutine(_context.Simulator.BotsManager.Generate());
            });

            // Wait for bots to be generated
            ThreadEvent.Wait(_lock);

            Dialogs.StopShowBotsDialog();
            ThreadEvent.Wait(_lock);

            _context.CanUseControls = true;
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.START_ROUND);
        }
    }
}
