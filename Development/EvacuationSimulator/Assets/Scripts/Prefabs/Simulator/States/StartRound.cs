using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class StartRound : SimulatorState
    {
        public StartRound(SimulatorContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            _context.SimulationRoundCounter++;
            
            _context.Simulator.Dispatcher.Schedule(() => 
            {
                _context.Simulator.simulationNumberUI.text = _context.SimulationRoundCounter.ToString();
                if (SimulationConfigurator.Instance.MiscellaneousSettings.Train)
                {
                    _context.Simulator.fireTraps.SetActive(true);
                }
            });

            _context.CanUseControls = true;
            SimulationConfigurator.Instance.CanSwitchCamera = true;
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.POSITION_BOTS);
        }
    }
}
