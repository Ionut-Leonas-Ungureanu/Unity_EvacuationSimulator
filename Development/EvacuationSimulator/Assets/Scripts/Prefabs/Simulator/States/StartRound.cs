﻿using Assets.Scripts.Prefabs.Simulator.States.Context;
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
            Debug.Log($">> Simulation round {_context.SimulationRoundCounter}");
            _context.Simulator.Dispatcher.Schedule(() => 
            {
                _context.Simulator.simulationNumberUI.text = _context.SimulationRoundCounter.ToString();
            });
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.POSITION_BOTS);
        }
    }
}