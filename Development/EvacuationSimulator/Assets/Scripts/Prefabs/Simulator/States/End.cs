using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Bot.States.Constants;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class End : SimulatorState
    {
        public End(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            _context.Simulator.StopSimulator();

            //_context.Simulator.Dispatcher.Schedule(() =>
            //{
            //    // Show Exit controlls
            //    _context.Simulator.resultsController.SetActive(true);
            //}).WaitOne();

            // Dispose bots
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                foreach (var bot in _context.Simulator.BotsManager.Bots)
                {
                    bot.Dispose();
                }
            });

            // Set new training data file
            if(File.Exists(Locals.DQN_NETWORK_FILE) && File.Exists(Locals.DQN_NETWORK_TRAINING_FILE))
            {
                File.Delete(Locals.DQN_NETWORK_FILE);
            }

            if (File.Exists(Locals.DQN_NETWORK_TRAINING_FILE))
            {
                File.Move(Locals.DQN_NETWORK_TRAINING_FILE, Locals.DQN_NETWORK_FILE);
            }

            // Write results
            _context.ResultsManager.Save();

            SimulationConfigurator.Instance.SimulationIsRunning = false;
            Debug.Log("End of simulation.");
        }

        protected override void SetNextState()
        {
            // End od simulation
        }
    }
}
