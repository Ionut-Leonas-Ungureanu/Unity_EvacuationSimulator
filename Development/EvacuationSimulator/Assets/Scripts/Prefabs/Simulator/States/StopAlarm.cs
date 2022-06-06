using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class StopAlarm : SimulatorState
    {
        public StopAlarm(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            // Stop alarm
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                SimulationConfigurator.Instance.SoundSettings.AlarmAudioSource.Stop();
            }).WaitOne();
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.CLOSE_DOORS);
        }
    }
}
