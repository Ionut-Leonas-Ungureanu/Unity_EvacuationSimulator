using Assets.Scripts.Prefabs.Simulator.States.Context;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class CloseDoors : SimulatorState
    {
        public CloseDoors(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            // Close doors
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.ROUND_CHECK);
        }
    }
}
