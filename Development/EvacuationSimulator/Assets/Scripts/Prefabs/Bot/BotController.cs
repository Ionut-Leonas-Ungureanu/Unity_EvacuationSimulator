using Assets.Scripts.Utils.Results;
using UnityEngine;
using Assets.Scripts.Prefabs.Bot.States.Context;
using Assets.Scripts.Utils;
using System.IO;
using System;
using Assets.Scripts.Utils.Processor;
using Assets.Scripts.Prefabs.Bot.Observers;
using System.Collections.Generic;
using Assets.Scripts.DesignPatterns.Singleton;

namespace Assets.Scripts.Prefabs.Bot
{
    class BotController : MonoBehaviour, IBotSubject, IDisposable
    {
        #region Properties

        private SkinnedMeshRenderer _headRenderer;
        private SkinnedMeshRenderer _suitRenderer;
        private CameraController _cameraController;
        private GameObject _botCamera;
        private Animator _animator;

        private BotContext _context;
        private Processor _processor;

        private string _gameObjectName;
        private object _lock;
        private object _botSubjectLock;

        public int Identifier { get; set; }
        public string Name => _gameObjectName;
        public double Distance => _context.Distance;
        public TimeSpan Time => _context.Time;
        public BotStatus Status => _context.Status;

        public float Speed { get; set; }
        public float Direction { get; set; } = 0;
        public float TargetDirection { get; set; } = 0;
        public bool IsActive => _context.HasToEscape;
        public bool HasToEscape
        {
            get => _context.HasToEscape;
            set => _context.HasToEscape = value;
        }
        public BotResult Result { get; set; }
        public UnityDispatcher Dispatcher { get; set; }
        public Dictionary<Guid, IBotObserver> Observers { get; private set; }
        #endregion

        #region Unity Methods

        public void Start()
        {
            _lock = new object();
            _botSubjectLock = new ThreadEvent();

            // Get name
            _gameObjectName = gameObject.name;

            Observers = new Dictionary<Guid, IBotObserver>();
            Dispatcher = new UnityDispatcher();

            if (Identifier == 0)
            {
                _context = new BotContext(this, SimulationConfigurator.Instance.MiscellaneousSettings.Train);
            }
            else
            {
                _context = new BotContext(this, false);
            }

            var task = new Action(() => { _context.State.Handle(); });
            var stopTask = new Action(() => { _context.Terminate = true; });
            _processor = new Processor(task, stopTask);

            Application.logMessageReceived += Application_logMessageReceived;
            Application.logMessageReceivedThreaded += Application_logMessageReceived;
            //EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

            var checkpoints = GameObject.FindGameObjectsWithTag("CheckPoint");
            _context.Checkpoints = new List<Vector3>();
            foreach (var checkpoint in checkpoints)
            {
                _context.Checkpoints.Add(checkpoint.transform.position);
            }

            // Get animator
            _animator = transform.GetComponent<Animator>();

            var geo = transform.GetChild(0).gameObject;
            var head = geo.transform.GetChild(0).gameObject;
            var suit = geo.transform.GetChild(1).gameObject;
            _headRenderer = head.GetComponent<SkinnedMeshRenderer>();
            _suitRenderer = suit.GetComponent<SkinnedMeshRenderer>();

            _cameraController = GameObject.Find("OfficeBounds").GetComponent<CameraController>();
            _botCamera = transform.Find("Camera").gameObject;

            _processor.Start();

            //Debug.Log($"{name} started");

            RegisterCamera();
            Show();
        }

        //private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        //{
        //    if (!EditorApplication.isPlaying && !EditorApplication.isPaused)
        //    {
        //        _processor.Stop();
        //    }
        //}

        public void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                using (var streamWriter = new StreamWriter("Application_LOG.log"))
                {
                    streamWriter.WriteLine($">> LOG TYPE: {type}");
                    streamWriter.WriteLine($">> CONDITION: {condition}");
                    streamWriter.WriteLine($">> STACK TRACE: {stackTrace}");
                }
            }
        }

        //private void OnEnable()
        //{
        //    // Get checkpoints
        //    var checkpoints = GameObject.FindGameObjectsWithTag("CheckPoint");
        //    // Get closest goal position
        //    //DistanceToGoal = Distance(checkpoints[0].transform.position);
        //    _context.GoalPosition = checkpoints[0].transform.position;

        //    //for (var i = 1; i < checkpoints.Length; ++i)
        //    //{
        //    //    var distance = Distance(checkpoints[i].transform.position);
        //    //    if (DistanceToGoal > distance)
        //    //    {
        //    //        DistanceToGoal = distance;
        //    //        GoalPosition = checkpoints[i].transform.position;
        //    //    }
        //    //}
        //}

        public void FixedUpdate()
        {
            // update bot position
            _context.BotPosition = transform.position;

            // Update movement direction
            UpdateDirection();

            // Execute tasks from other threads (1 task per frame)
            Dispatcher.Execute();
        }

        private void UpdateDirection()
        {
            _animator.SetFloat("Speed", Speed);

            lock (_lock)
            {
                if (TargetDirection > Direction)
                {
                    Direction += 0.1f;
                }
                if (TargetDirection < Direction)
                {
                    Direction -= 0.1f;
                }

                _animator.SetFloat("Direction", Direction);
            }
        }

        private void OnDestroy()
        {
            _processor.Stop();
        }

        #endregion

        #region Methods

        //public void Activate()
        //{
        //    _processor.Start();
        //}

        //public void Deactivate()
        //{
        //    _processor.Stop();
        //}

        public void StopMoving()
        {
            lock (_lock)
            {
                Speed = 0f;
                Direction = 0f;
                TargetDirection = 0f;
                Debug.Log("StopMoving");
            }
        }

        public void StartMoving()
        {
            lock (_lock)
            {
                Speed = 1f;
                Direction = 0f;
                TargetDirection = 0f;
                Debug.Log("StartMoving");
            }
        }

        public void UpdateDirection(float target)
        {
            lock (_lock)
            {
                TargetDirection = target;
            }
        }

        public void Hide()
        {
            _context.Bot._headRenderer.enabled = false;
            _context.Bot._suitRenderer.enabled = false;
        }

        public void Show()
        {
            _context.Bot._headRenderer.enabled = true;
            _context.Bot._suitRenderer.enabled = true;
        }

        public void UpdateObserver(RunResultsContainer container)
        {
            //Brain.SaveNetwork(Locals.NEURAL_NETWORK_DATA_FILE);
            Result = new BotResult
            {
                Name = gameObject.name,
                //HealthPoints = HealthPoints.ToString(),
                //Time = $"{_time.Hours}:{_time.Minutes}:{_time.Seconds}.{_time.Milliseconds}",
                //Distance = DistanceTravelled.ToString(),
                //Reward = TotalReward.ToString()
            };
            container.Add(Result, Identifier);
        }

        public void RegisterCamera()
        {
            _cameraController.Register(_botCamera);
        }

        public void UnregisterCamera()
        {
            _cameraController.Unregister(_botCamera);
        }

        public void Subscribe(IBotObserver observer)
        {
            lock (_botSubjectLock)
            {
                if (!Observers.ContainsKey(observer.Id))
                {
                    Observers.Add(observer.Id, observer);
                }
            }
        }

        public void UnSubscribe(IBotObserver observer)
        {
            lock (_botSubjectLock)
            {
                if (Observers.ContainsKey(observer.Id))
                {
                    Observers.Remove(observer.Id);
                }
            }
        }

        public void Notify()
        {
            lock (_botSubjectLock)
            {
                foreach (var item in Observers)
                {
                    item.Value.Update();
                }
            }
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (_context?.NavigationPath == null)
            {
                return;
            }

            foreach (var node in _context.NavigationPath)
            {
                if (node != null)
                {
                    if (node.Walkable)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawCube(node.Position, Vector3.one * (0.2f * 2 - 0.1f));
                }
            }
        }

        public void Dispose()
        {
            _processor.Stop();
            UnregisterCamera();
        }
    }
}
