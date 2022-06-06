using Assets.Scripts.Prefabs.Simulator.States.Context;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class StartFire : SimulatorState
    {
        public StartFire(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            // Start spreading fire
            _context.SpreadFireCoroutine = _context.Simulator.FireManager.SpreadFire();
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                _context.Simulator.StartCoroutine(_context.SpreadFireCoroutine);
            }).WaitOne();
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.START_ALARM);
        }
    }
}
