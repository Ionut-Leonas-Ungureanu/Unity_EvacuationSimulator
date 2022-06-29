using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Simulator.States.Context;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.AStar;
using Assets.Scripts.Utils.Processor;
using System;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Simulator
{
    class Simulator : MonoBehaviour
    {
        private SimulatorContext _context;
        private Processor _processor;

        private bool _activeControls;
        private float _fpsElapsedTime;

        internal FireManager FireManager { get; set; }
        internal BotsManager BotsManager { get; set; }

        #region Game objects
        public GameObject bounds;
        public GameObject botPrefab;
        public GameObject resultsCanvas;
        public GameObject resultsContainer;
        public GameObject resultsController;
        public GameObject dialogs;
        public GameObject controls;
        public GameObject navigation;
        public GameObject fireManager;
        public GameObject fireTraps;
        public AudioSource audioSource;
        public TMPro.TextMeshProUGUI simulationNumberUI;
        public TMPro.TextMeshProUGUI fpsUI;
        #endregion

        public Func<GameObject, Vector3, GameObject> InstantiateAtPositionGameObject = new Func<GameObject, Vector3, GameObject>((objectToInstantiate, position) =>
        {
            return Instantiate(objectToInstantiate, position, Quaternion.identity);
        });

        public Func<GameObject, GameObject> InstantiateGameObject = new Func<GameObject, GameObject>((objectToInstantiate) =>
        {
            return Instantiate(objectToInstantiate);
        });

        public Action<GameObject> DestroyGameObject = new Action<GameObject>((objectToDestroy) =>
        {
            Destroy(objectToDestroy);
        });

        public UnityDispatcher Dispatcher { get; set; }

        public void Start()
        {
            Dispatcher = new UnityDispatcher();
            BotsManager = new BotsManager(botPrefab, InstantiateAtPositionGameObject, SimulationConfigurator.Instance.BotsSettings.NumberBots);
            FireManager = fireManager.GetComponent<FireManager>();

            _context = new SimulatorContext(this);
            var task = new Action(() => _context.State.Handle());
            var stopTask = new Action(() => { });
            _processor = new Processor(task, stopTask);

            SimulationConfigurator.Instance.SoundSettings.AlarmAudioSource = audioSource;

            _activeControls = false;
            controls.SetActive(_activeControls);

            _processor.Start();
        }

        private void Update()
        {
            UpdateFPS();

            if (_context.CanUseControls && Input.GetKeyDown(KeyCode.Escape))
            {
                controls.SetActive(!controls.activeSelf);
                if (_context.IsResultActive)
                {
                    resultsCanvas.SetActive(!resultsCanvas.activeSelf);
                    resultsController.SetActive(!resultsController.activeSelf);
                }
            }
        }

        private void FixedUpdate()
        {
            Dispatcher.Execute();
        }

        public void StopSimulator()
        {
            _processor.Stop();
        }

        private void UpdateFPS()
        {
            _fpsElapsedTime += Time.deltaTime;
            if (_fpsElapsedTime > 1)
            {
                fpsUI.text = $"{(1f / Time.deltaTime):F0}";
                _fpsElapsedTime = 0;
            }
        }

        //private void OnDrawGizmos()
        //{
        //    if (!Navigator.IsGenerated)
        //    {
        //        return;
        //    }

        //    foreach (var node in Navigator.Grid)
        //    {
        //        if (node != null)
        //        {
        //            Gizmos.color = Color.red;
        //            Gizmos.DrawCube(node.Position, Vector3.one * (0.2f * 2 - 0.1f));
        //        }
        //    }
        //}
    }
}
