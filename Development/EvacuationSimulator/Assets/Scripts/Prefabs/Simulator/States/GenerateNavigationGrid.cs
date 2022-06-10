using Assets.Scripts.Prefabs.Simulator.States.Context;
using Assets.Scripts.Utils;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class GenerateNavigationGrid : SimulatorState
    {
        private readonly object _lock;

        public GenerateNavigationGrid(SimulatorContext context) : base(context)
        {
            _lock = new object();
        }

        protected override void HandleState()
        {
            Dialogs.OnDialogClose = () =>
            {
                ThreadEvent.Pulse(_lock);
            };

            // Start generating the grid
            _context.Simulator.Dispatcher.Schedule(() =>
            {
                var grid = _context.Simulator.navigation.GetComponent<CustomGrid>();
                grid.OnGridGenerated = () =>
                {
                    ThreadEvent.Pulse(_lock);
                };
                _context.Simulator.StartCoroutine(Dialogs.ShowNavigationDialog());
                _context.Simulator.StartCoroutine(grid.Generate());
            });

            // Wait to finish
            ThreadEvent.Wait(_lock);

            Dialogs.StopShowNavigationDialog();
            ThreadEvent.Wait(_lock);
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.GENERATE_FIRE);
        }
    }
}
