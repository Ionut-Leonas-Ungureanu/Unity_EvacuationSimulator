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

            // Dispose bots
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                var cameraController = GameObject.Find("OfficeBounds").GetComponent<CameraController>();
                cameraController.ShowMain();

                foreach (var bot in _context.Simulator.BotsManager.Bots)
                {
                    bot.Dispose();
                    _context.Simulator.DestroyGameObject(bot.gameObject);
                }

                foreach(var fireNode in _context.Simulator.FireManager.FireNodes)
                {
                    _context.Simulator.DestroyGameObject(fireNode);
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
            SimulationConfigurator.Instance.CanSwitchCamera = false;
        }

        protected override void SetNextState()
        {
            // End od simulation
        }
    }
}
