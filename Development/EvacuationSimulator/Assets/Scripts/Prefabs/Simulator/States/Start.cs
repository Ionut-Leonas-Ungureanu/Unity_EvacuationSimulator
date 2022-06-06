using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class Start : SimulatorState
    {
        public Start(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            SimulationConfigurator.Instance.SimulationIsRunning = true;

            // Check Network data directory
            Directory.CreateDirectory("Network_Files");
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.GENERATE_NAVIGATION_GRID);
        }
    }
}
