using Assets.Scripts.Utils;

namespace Assets.Scripts.DesignPatterns.Singleton
{
    public class SimulationConfigurator
    {
        public static SimulationConfigurator Instance => SimulationConfiguratorNested.instance;

        public BotsSettings BotsSettings { get; set; } = new BotsSettings();
        public SimulationSettings SimulationSettings { get; set; } = new SimulationSettings();
        public FireSettings FireSettings { get; set; } = new FireSettings();
        public SoundSettings SoundSettings { get; set; } = new SoundSettings();
        public MiscellaneousSettings MiscellaneousSettings { get; set; } = new MiscellaneousSettings();

        public bool SimulationIsRunning { get; set; }
        public bool StopSimulation { get; set; }
        public bool CanSwitchCamera { get; set; } = false;

        private static class SimulationConfiguratorNested
        {
            static SimulationConfiguratorNested() { }

            internal static readonly SimulationConfigurator instance = new SimulationConfigurator();
        }
    }
}
