using DeepQNetworkWrapper;
using System.Collections.Generic;
using System.Threading;
using System;
using Assets.Scripts.Prefabs.Bot.States.Constants;
using System.IO;
using Assets.Scripts.Utils.Experience;
using UnityEngine;
using Assets.Scripts.Utils.AStar;
using System.Diagnostics;
using Assets.Scripts.Utils.Processor;
using Assets.Scripts.DesignPatterns.Singleton;

namespace Assets.Scripts.Prefabs.Bot.States.Context
{
    public enum BotStatus
    {
        SAFE,
        DEAD,
        TRAPPED
    }

    class BotContext
    {


        #region Properties

        public Guid identifier = Guid.NewGuid();
        public object Lock { get; set; }

        private object _botPositionLock;
        private ReaderWriterLock _botReaderWriterLock;
        private Vector3 _botPosition;
        public Vector3 BotPosition
        {
            get
            {
                try
                {
                    _botReaderWriterLock.AcquireReaderLock(Timeout.Infinite);
                    return _botPosition;
                }
                finally
                {
                    _botReaderWriterLock.ReleaseReaderLock();
                }
            }
            set
            {
                try
                {
                    _botReaderWriterLock.AcquireWriterLock(Timeout.Infinite);
                    _botPosition = value;
                }
                finally
                {
                    _botReaderWriterLock.ReleaseWriterLock();
                }

                //lock (_botPositionLock)
                //{
                //    Monitor.PulseAll(_botPositionLock);
                //}
            }
        }
        public BotController Bot { get; private set; }
        public Vector3 LastBotPosition { get; set; }
        public Vector3 GoalPosition { get; set; }
        public uint ObservationsAroundGoal { get; set; }
        public List<Vector3> Checkpoints { get; set; }
        public BotState State { get; set; }
        public Dictionary<BotStateType, BotState> States { get; set; }

        #region Navigation

        private ProximityValidator _rectComputer;
        public object NavigationLock { get; set; }
        public object NavigationConsumerLock { get; set; }
        public bool RunNavigationConsumerThread { get; set; }
        public List<Node> NavigationPath { get; set; }
        public Processor PathConsumer { get; set; }

        #endregion


        #region DQN

        public DeepQWrapper Brain { get; set; }
        public int Action { get; set; }
        public double[] InitialObservation { get; set; }
        public double[] Observation { get; set; }
        public ExperienceBuffer<ExperienceContainer> ExperienceBuffer { get; set; }

        #endregion

        private bool _hasToEscape;
        public bool HasToEscape
        {
            get => _hasToEscape;
            set
            {
                _hasToEscape = value;
                if (_hasToEscape)
                {
                    lock (Lock)
                    {
                        Monitor.Pulse(Lock);
                    }
                }
            }
        }
        public bool IsDead { get; set; }
        public bool IsCheckpointReached { get; set; }
        public bool IsTrapped { get; set; }
        public bool IsColliding { get; set; }
        public bool IsOutsideOfBuilding { get; set; }

        #region Result info
        public double Distance { get; set; }
        public TimeSpan Time { get; set; }
        public BotStatus Status { get; set; }
        private Stopwatch _stopwatch;
        #endregion

        #region Simulation related

        private bool _terminate;
        public bool Terminate
        {
            get => _terminate;
            set
            {
                _terminate = value;
                if (_terminate)
                {
                    lock (Lock)
                    {
                        Monitor.Pulse(Lock);
                    }
                }
            }
        }

        public bool IsTrainingTheNetwork { get; set; } = false;
        public uint CollectedCount { get; set; }

        #endregion

        #endregion

        public BotContext(BotController botController, bool train)
        {
            Bot = botController;
            IsTrainingTheNetwork = train;

            _stopwatch = new Stopwatch();
            _botPositionLock = new object();
            _botReaderWriterLock = new ReaderWriterLock();
            _rectComputer = new ProximityValidator(Bot);

            Lock = new object();
            NavigationLock = new object();
            NavigationConsumerLock = new object();

            //UnityEngine.Debug.Log($"{Bot.Name} trains: {IsTrainingTheNetwork}");

            InitializeStates();
            InitializeDeepQNetwork();
            ExperienceBuffer = new ExperienceBuffer<ExperienceContainer>();
        }

        #region Methods

        public void Reset()
        {
            lock (_botPositionLock)
            {
                Monitor.PulseAll(_botPositionLock);
            }

            IsDead = false;
            IsCheckpointReached = false;
            IsTrapped = false;
            Distance = 0;
            CollectedCount = 0;
        }

        public void SetState(BotStateType stateType)
        {
            State = States[stateType];
        }

        public Vector3 GetBotPosition()
        {
            Vector3 position = Vector3.zero;
            position.x = Bot.transform.position.x;
            position.y = Bot.transform.position.y;
            position.z = Bot.transform.position.z;
            return position;
        }

        public void AddDistanceWalked(Vector3 newBotPosition)
        {

            Distance += Vector3.Distance(newBotPosition, LastBotPosition);
            LastBotPosition = newBotPosition;
        }

        public void StartMeasuringEscapingTime()
        {
            _stopwatch.Start();
        }

        public void StopMeasuringEscapingTime()
        {
            _stopwatch.Stop();
            Time = _stopwatch.Elapsed;
            _stopwatch.Reset();
        }

        #region Navigation

        public void FindPath(Vector3 position)
        {
            // Get closest checkpoint
            var minDistance = Vector3.Distance(position, Checkpoints[0]);
            GoalPosition = Checkpoints[0];
            for (var i = 1; i < Checkpoints.Count; ++i)
            {
                var newDistance = Vector3.Distance(position, Checkpoints[i]);
                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    GoalPosition = Checkpoints[i];
                }
            }

            // Calculate path
            NavigationPath = Navigator.FindPath(position, GoalPosition);
        }

        public List<Node> ClearCollinears(List<Node> list)
        {
            var result = new List<Node>();

            var startIndex = 0;
            var inBetweenIndex = 1;
            var endIndex = 2;

            result.Add(list[startIndex]);

            while (endIndex < list.Count)
            {
                if (Vector3.Cross(list[endIndex].Position - list[startIndex].Position, list[endIndex].Position - list[inBetweenIndex].Position) != Vector3.zero)
                {
                    result.Add(list[inBetweenIndex]);
                }

                startIndex = inBetweenIndex;
                inBetweenIndex = endIndex;
                endIndex++;
            }

            return result;
        }

        public void ConsumeNavigationPath()
        {
            Vector3 firstNodePosition = Vector3.zero;
            Vector3 secondNodePosition = Vector3.zero;

            lock (NavigationLock)
            {
                if (NavigationPath == null || NavigationPath.Count <= 2)
                {
                    //UnityEngine.Debug.Log("Path consumer thread has ended.");
                    return;
                }

                firstNodePosition = NavigationPath[0].Position;
                secondNodePosition = NavigationPath[1].Position;
            }


            var botPosition = BotPosition;
            var distance = Vector3.Distance(botPosition, firstNodePosition);

            //if (distance > 6)
            //{
            //    UnityEngine.Debug.Log("Distance > 6. The path is recalculated.");
            //    lock (NavigationLock)
            //    {
            //        FindPath(botPosition);
            //    }
            //    return;
            //}

            var direction = secondNodePosition - firstNodePosition;

            if (_rectComputer.IsInProximity(botPosition, firstNodePosition, direction, distance)
                && firstNodePosition.y > botPosition.y)
            {
                CollectedCount++;

                lock (NavigationLock)
                {
                    NavigationPath.RemoveAt(0);

                    // Check if following nodes are unwalkable
                    if (NavigationPath.Count > 8)
                    {
                        for (var i = 7; i >= 0; --i)
                        {
                            if (!NavigationPath[i].Walkable)
                            {
                                UnityEngine.Debug.Log("The path is recalculated.");
                                FindPath(botPosition);
                                break;
                            }
                        }
                    }
                }
            }

            //lock (_botPositionLock)
            //{
            //    Monitor.Wait(_botPositionLock);
            //}

            Thread.Sleep(SimulationConfigurator.Instance.MiscellaneousSettings.ClearPathSpeed);

            //UnityEngine.Debug.Log("Path consumer thread has ended.");
        }

        #endregion

        private void InitializeStates()
        {
            States = new Dictionary<BotStateType, BotState>();
            foreach (var stateType in (BotStateType[])Enum.GetValues(typeof(BotStateType)))
            {
                States.Add(stateType, BotStatesFactory.MakeBotState(stateType, this));
            }

            State = States[BotStateType.START];
        }

        #region Deep Q Network

        public void SaveNeuralNetwork()
        {
            Brain.SaveNetwork(Locals.DQN_NETWORK_TRAINING_FILE);
        }

        private void InitializeDeepQNetwork()
        {
            // Set up the neural network properties
            NeuralNetworkWrapper neuralNetwork;
            neuralNetwork = new NeuralNetworkWrapper(Locals.DQN_LEARNING_RATE);

            if (!IsTrainingTheNetwork && File.Exists(Locals.DQN_NETWORK_FILE))
            {
                neuralNetwork.Load(Locals.DQN_NETWORK_FILE);
                // Set up the DQN properties
                Brain = new DeepQWrapper(neuralNetwork);

                // Set experience
                Brain.SetExploration(0);
                Brain.SetMinimumExploration(0);
                Brain.SetExplorationDecay(0);
                Brain.SetGamma(Locals.DQN_GAMMA);
                Brain.SetTrainEpochs(Locals.DQN_TRAIN_EPOCHS);
                Brain.SetReplayBatchSize(Locals.DQN_REPLAY_BATCH_SIZE);
                Brain.SetExperienceMemorySize(Locals.DQN_EXPERIENCE_MEMORY_SIZE);
                Brain.SetUpdateTargetWeightsAfterSteps(Locals.DQN_UPDATE_TARGET_STEPS);
            }
            else
            {
                neuralNetwork.AddLayer(Locals.DQN_HIDDEN_NO_NEURONS, (uint)Locals.OBSERVATION_SIZE, Locals.DQN_HIDDEN_LAYER_FUNCTION);
                //neuralNetwork.AddLayer(Locals.DQN_HIDDEN_NO_NEURONS / 2, Locals.DQN_HIDDEN_NO_NEURONS, Locals.DQN_HIDDEN_LAYER_FUNCTION);
                neuralNetwork.AddLayer((uint)Locals.ACTION_SIZE, Locals.DQN_HIDDEN_NO_NEURONS, Locals.DQN_OUTPUT_LAYER_FUNCTION);

                // Set up the DQN properties
                Brain = new DeepQWrapper(neuralNetwork);

                // Set experience
                Brain.SetExploration(Locals.DQN_EXPLORATION);
                Brain.SetMinimumExploration(Locals.DQN_MINIMUM_EXPLORATION);
                Brain.SetExplorationDecay(Locals.DQN_EXPLORATION_DECAY);
                Brain.SetGamma(Locals.DQN_GAMMA);
                Brain.SetTrainEpochs(Locals.DQN_TRAIN_EPOCHS);
                Brain.SetReplayBatchSize(Locals.DQN_REPLAY_BATCH_SIZE);
                Brain.SetExperienceMemorySize(Locals.DQN_EXPERIENCE_MEMORY_SIZE);
                Brain.SetUpdateTargetWeightsAfterSteps(Locals.DQN_UPDATE_TARGET_STEPS);
            }
        }

        #endregion

        #endregion
    }
}
