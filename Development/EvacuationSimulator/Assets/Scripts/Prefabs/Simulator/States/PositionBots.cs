using Assets.Scripts.Prefabs.Simulator.States.Context;
using System;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class PositionBots : SimulatorState
    {
        public PositionBots(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                _context.Simulator.BotsManager.Reset();
            }).WaitOne();
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.START_FIRE);
        }
    }
}
