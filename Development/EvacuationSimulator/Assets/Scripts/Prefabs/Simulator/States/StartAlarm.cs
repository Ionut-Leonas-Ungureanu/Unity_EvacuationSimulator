using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class StartAlarm : SimulatorState
    {
        public StartAlarm(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            // Start alarm
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                SimulationConfigurator.Instance.SoundSettings.AlarmAudioSource.Play();
            }).WaitOne();
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.OPEN_DOORS);
        }
    }
}
