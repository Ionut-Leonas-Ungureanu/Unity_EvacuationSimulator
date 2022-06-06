using Assets.Scripts.Prefabs.Simulator.States.Context;
using Assets.Scripts.Utils;
using System.Threading;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class GenerateFire : SimulatorState
    {
        private readonly object _lock; 

        public GenerateFire(SimulatorContext context) : base(context)
        {
            _lock = new object();
            _context.Simulator.FireManager.OnGenerated += FireManager_OnGenerated;
        }

        protected override void HandleState()
        {
            Dialogs.OnDialogClose = () =>
            {
                ThreadEvent.Pulse(_lock);
            };

            // Generate fire
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                _context.Simulator.StartCoroutine(Dialogs.ShowFireDialog());
                _context.Simulator.StartCoroutine(_context.Simulator.FireManager.Generate());
            });

            // Wait for fire to be prepared
            ThreadEvent.Wait(_lock);

            Dialogs.StopShowFireDialog();
            ThreadEvent.Wait(_lock);
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.GENERATE_BOTS);
        }

        private void FireManager_OnGenerated()
        {
            ThreadEvent.Pulse(_lock);
        }
    }
}
