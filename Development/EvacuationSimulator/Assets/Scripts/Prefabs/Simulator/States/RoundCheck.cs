using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class RoundCheck : SimulatorState
    {

        public RoundCheck(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {

        }

        protected override void SetNextState()
        {
            if(_context.SimulationRoundCounter == _context.NumberOfTotalRounds)
            {
                _context.SetState(SimulatorStateType.END);
                return;
            }

            if (SimulationConfigurator.Instance.StopSimulation)
            {
                _context.SetState(SimulatorStateType.END);
                SimulationConfigurator.Instance.StopSimulation = false;
                return;
            }

            _context.SetState(SimulatorStateType.START_ROUND);
        }
    }
}
