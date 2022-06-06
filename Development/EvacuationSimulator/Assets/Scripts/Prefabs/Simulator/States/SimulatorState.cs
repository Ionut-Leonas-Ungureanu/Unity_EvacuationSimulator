using Assets.Scripts.Prefabs.Simulator.States.Context;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    abstract class SimulatorState
    {
        #region Properties

        protected SimulatorContext _context;

        #endregion

        public SimulatorState(SimulatorContext context)
        {
            _context = context;
        }

        public virtual void Handle()
        {
            // Execute current state
            HandleState();
            // Set which state is next
            SetNextState();
        }

        protected abstract void HandleState();
        protected abstract void SetNextState();
    }
}
