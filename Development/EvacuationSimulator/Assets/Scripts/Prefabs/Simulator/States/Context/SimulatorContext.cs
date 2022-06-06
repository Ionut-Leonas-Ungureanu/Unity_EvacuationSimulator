using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Utils.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Simulator.States.Context
{
    class SimulatorContext
    {
        #region Properties

        public uint NumberOfTotalRounds { get; set; } = (uint)SimulationConfigurator.Instance.SimulationSettings.NumberOfRuns;
        public uint SimulationRoundCounter { get; set; }

        public Simulator Simulator { get; set; }
        public ResultsManager ResultsManager { get; set; }

        public SimulatorState State { get; set; }
        public Dictionary<SimulatorStateType, SimulatorState> States { get; set; }

        public GameObject OfficePositions { get; set; }
        public GameObject TrainingPositions { get; set; }
        public IEnumerator SpreadFireCoroutine { get; set; }

        public bool CanUseControls { get; set; }
        public bool IsResultActive { get; set; }


        #endregion

        public SimulatorContext(Simulator simulator)
        {
            Simulator = simulator;
            ResultsManager = new ResultsManager();

            InitializeStates();
        }

        #region Methods

        private void InitializeStates()
        {
            States = new Dictionary<SimulatorStateType, SimulatorState>();
            foreach (var stateType in (SimulatorStateType[])Enum.GetValues(typeof(SimulatorStateType)))
            {
                States.Add(stateType, SimulatorStatesFactory.MakeSimulatorState(stateType, this));
            }

            State = States[SimulatorStateType.START];
        }

        public void SetState(SimulatorStateType stateType)
        {
            State = States[stateType];
        }

        #endregion
    }
}
