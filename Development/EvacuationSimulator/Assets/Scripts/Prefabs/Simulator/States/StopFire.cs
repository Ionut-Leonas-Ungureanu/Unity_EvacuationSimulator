using Assets.Scripts.Prefabs.Simulator.States.Context;
using Assets.Scripts.Utils.AStar;
using System.Threading;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class StopFire : SimulatorState
    {
        private readonly object _lock;

        public StopFire(SimulatorContext context) : base(context)
        {
            _lock = new object();
            _context.Simulator.FireManager.OnFireNodesDisabled += FireManager_OnFireNodesDisabled;
        }

        protected override void HandleState()
        {
            _context.Simulator.Dispatcher.Schedule(() => 
            {
                _context.Simulator.StopCoroutine(_context.SpreadFireCoroutine);
                _context.Simulator.StartCoroutine(_context.Simulator.FireManager.DisableFireNodes());
            });

            lock (_lock)
            {
                Monitor.Wait(_lock);
            }

            Navigator.ResetGrid();
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.STOP_ALARM);
        }

        private void FireManager_OnFireNodesDisabled()
        {
            lock (_lock)
            {
                Monitor.Pulse(_lock);
            }
        }
    }
}
