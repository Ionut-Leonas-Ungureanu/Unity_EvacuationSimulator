using Assets.Scripts.Prefabs.Simulator.States.Context;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class OpenDoors : SimulatorState
    {
        public OpenDoors(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            // Open doors
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.OBSERVE_BOTS);
        }
    }
}
