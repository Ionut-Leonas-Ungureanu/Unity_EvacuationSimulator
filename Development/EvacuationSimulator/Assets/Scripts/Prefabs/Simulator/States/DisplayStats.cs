using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using Assets.Scripts.Utils.Results;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    class DisplayStats : SimulatorState
    {
        private List<GameObject> _resultContainers;

        public DisplayStats(SimulatorContext context) : base(context)
        {
            _resultContainers = new List<GameObject>();
        }

        protected override void HandleState()
        {
            _context.Simulator.Dispatcher.Schedule(ShowResults).WaitOne();
            Thread.Sleep((int)SimulationConfigurator.Instance.SimulationSettings.WaitToDisplayResults * 1000);
            if (_context.SimulationRoundCounter != SimulationConfigurator.Instance.SimulationSettings.NumberOfRuns)
            {
                _context.Simulator.Dispatcher.Schedule(DeleteResults).WaitOne();
            }
        }

        protected override void SetNextState()
        {
            _context.SetState(SimulatorStateType.STOP_FIRE);
        }

        private void ShowResults()
        {
            var container = new RunResultsContainer(_context.Simulator.BotsManager.Bots.Length, _context.SimulationRoundCounter);
            foreach(var bot in _context.Simulator.BotsManager.Bots)
            {
                var result = new BotResult
                {
                    Name = bot.Name,
                    Distance = bot.Distance.ToString("F2"),
                    Time = bot.Time.ToString(@"hh\:mm\:ss"),
                    Status = bot.Status.ToString()
                };
                // Remember the result 
                container.Add(result, bot.Identifier);
                _context.ResultsManager.Add(container);

                // Show on GUI
                var resultContainer = _context.Simulator.InstantiateGameObject(_context.Simulator.resultsContainer);

                // Write Name
                var nameText = resultContainer.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
                nameText.text = result.Name;
                // Write Distance
                var distanceText = resultContainer.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
                distanceText.text = result.Distance;
                // Write Time
                var timeText = resultContainer.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>();
                timeText.text = result.Time;
                // Write Status
                var statusText = resultContainer.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>();
                statusText.text = result.Status;
                // Set size
                var resultsRectTransform = _context.Simulator.resultsCanvas.transform.GetComponent<RectTransform>();
                resultsRectTransform.sizeDelta = new Vector3(resultsRectTransform.sizeDelta.x, resultsRectTransform.sizeDelta.y + 60);
                resultsRectTransform.position = new Vector3(resultsRectTransform.position.x, resultsRectTransform.position.y - 30, resultsRectTransform.position.z);

                resultContainer.transform.SetParent(_context.Simulator.resultsCanvas.transform);
                _resultContainers.Add(resultContainer);
            }

            _context.Simulator.controls.SetActive(false);
            _context.Simulator.resultsCanvas.SetActive(true);
            _context.Simulator.resultsController.SetActive(true);
            _context.IsResultActive = true;
        }

        private void DeleteResults()
        {
            _context.Simulator.resultsController.SetActive(false);
            _context.Simulator.resultsCanvas.SetActive(false);
            _context.IsResultActive = false;

            // Reset size
            var resultsRectTransform = _context.Simulator.resultsCanvas.transform.GetComponent<RectTransform>();
            resultsRectTransform.sizeDelta = new Vector3(resultsRectTransform.sizeDelta.x, resultsRectTransform.sizeDelta.y - 60);
            resultsRectTransform.position = new Vector3(resultsRectTransform.position.x, resultsRectTransform.position.y + 30, resultsRectTransform.position.z);

            foreach (var resultContainer in _resultContainers)
            {
                _context.Simulator.DestroyGameObject(resultContainer);
            }
            _resultContainers.Clear();
        }
    }
}
