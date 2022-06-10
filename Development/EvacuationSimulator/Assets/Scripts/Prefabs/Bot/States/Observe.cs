using Assets.Scripts.Prefabs.Bot.States.Constants;
using Assets.Scripts.Prefabs.Bot.States.Context;
using Assets.Scripts.Utils.AStar;
using Assets.Scripts.Utils.Experience;
using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Observe : BotState
    {
        #region Properties

        private Action _storeExperience;

        private Vector3 _raysStartingPoint;
        private Vector3 _raysMiddlePoint;
        private Vector3 _startCapsuleColliderConstant;
        private Vector3 _endCapsuleColliderConstant;

        private readonly float _raysColliderDistanceToPoints;
        private readonly float _radiusCapsuleCollider;
        private readonly int _botLayerMask = ~(1 << 10);
        private uint _lastCollectedCount;

        #endregion

        public Observe(BotContext context) : base(context)
        {
            _storeExperience = FirstObserve;

            _raysStartingPoint = new Vector3(0, 0.86f, 0);
            _raysMiddlePoint = _raysStartingPoint - new Vector3(0, 0.1f, 0);
            _raysColliderDistanceToPoints = 1.83f / 2 - 0.25f / 2;
            var center = new Vector3(0, 0.95f, 0);
            var height = 1.7f;
            _radiusCapsuleCollider = 0.29f;
            var distanceToPoints = height / 2 - _radiusCapsuleCollider;
            _startCapsuleColliderConstant = center + Vector3.up * distanceToPoints;
            _endCapsuleColliderConstant = center - Vector3.up * distanceToPoints;
        }

        protected override void HandleState()
        {
            GetObservation();

            _storeExperience();

            //Array.Copy(_context.InitialObservation, _context.Observation, _context.InitialObservation.Length);
            _context.InitialObservation = _context.Observation;
        }

        protected override void SetNextState()
        {
            if (_context.IsCheckpointReached)
            {
                _context.SetState(BotStateType.CHECKPOINT);
            }
            else if (_context.IsDead)
            {
                _context.SetState(BotStateType.DEAD);
            }
            else if (_context.NavigationPath == null || _context.NavigationPath.Count == 0)
            {
                _context.SetState(BotStateType.TRAPPED);
                _context.IsTrapped = true;
            }
            else
            {
                _context.SetState(BotStateType.PREDICT);
                return;
            }

            _storeExperience = FirstObserve;
        }

        #region Methods

        private void GetObservation()
        {
            _context.Observation = new double[Locals.OBSERVATION_SIZE];
            float angle = 0;
            float distance = 0;

            _context.Bot.Dispatcher.Schedule(() =>
            {
                lock (_context.NavigationLock)
                {
                    if (_context.NavigationPath != null && _context.NavigationPath.Count >= 2)
                    {
                        // Calculate angles
                        var direction = _context.NavigationPath[0].Position - _context.Bot.transform.position;
                        angle = Vector3.SignedAngle(_context.Bot.transform.forward, direction, Vector3.up);

                        // Calculate distances
                        distance = Vector3.Distance(_context.Bot.transform.position, _context.NavigationPath[0].Position);
                    }
                }

                // Get proximities
                // Create commands for horizontal rays
                var middlePoint = _context.Bot.transform.position + _raysMiddlePoint;
                var bottomPoint = _context.Bot.transform.position + _raysStartingPoint - Vector3.up * _raysColliderDistanceToPoints;

                var results = new NativeArray<RaycastHit>(Locals.OBSERVATION_TOTAL_NUMBER_OF_RAYS, Allocator.TempJob);
                var commands = new NativeArray<RaycastCommand>(Locals.OBSERVATION_TOTAL_NUMBER_OF_RAYS, Allocator.TempJob);

                for (var i = 0; i < Locals.OBSERVATION_NUMBER_OF_RAYS; ++i)
                {
                    var direction = Quaternion.AngleAxis(Locals.OBSERVATION_START_ANGLE_OF_RAYS + Locals.OBSERVATION_OFFSET_ANGLE_OF_RAYS * i, _context.Bot.transform.up) *
                        _context.Bot.transform.forward;

                    commands[i] = new RaycastCommand(middlePoint, direction, distance: Locals.OBSERVATION_MAX_DISTANCE_OF_RAYS, layerMask: _botLayerMask);
                    commands[Locals.OBSERVATION_NUMBER_OF_RAYS + i] = new RaycastCommand(bottomPoint, direction, distance: Locals.OBSERVATION_MAX_DISTANCE_OF_RAYS, layerMask: _botLayerMask);
                }

                var handle = RaycastCommand.ScheduleBatch(commands, results, 1);
                handle.Complete();

                // Write proximities to observation
                for (var i = 0; i < Locals.OBSERVATION_TOTAL_NUMBER_OF_RAYS; ++i)
                {
                    // Get distance and fire 
                    if (results[i].collider != null)
                    {
                        _context.Observation[i] = results[i].distance;
                        switch (results[i].collider.tag)
                        {
                            case "Fire":
                                _context.Observation[Locals.OBSERVATION_TOTAL_NUMBER_OF_RAYS + i] = 2;
                                break;
                            case "Bot":
                                _context.Observation[Locals.OBSERVATION_TOTAL_NUMBER_OF_RAYS + i] = 1;
                                break;
                            default:
                                _context.Observation[Locals.OBSERVATION_TOTAL_NUMBER_OF_RAYS + i] = 0;
                                break;
                        }
                    }
                    else
                    {
                        _context.Observation[i] = Locals.OBSERVATION_MAX_DISTANCE_OF_RAYS;
                    }
                }

                // Free buffers
                results.Dispose();
                commands.Dispose();

                // Prepare data for collision
                var startCapsule = _context.Bot.transform.position + _startCapsuleColliderConstant;
                var endCapsule = _context.Bot.transform.position + _endCapsuleColliderConstant;
                var colliders = Physics.OverlapCapsule(startCapsule, endCapsule, _radiusCapsuleCollider);

                // Get collision
                var collision = 0;
                foreach (var collider in colliders)
                {
                    if (collider.name != _context.Bot.Name)
                    {
                        switch (collider.tag)
                        {
                            case "Bot":
                                collision |= 1;
                                break;
                            case "Fire":
                                _context.IsDead = true;
                                collision |= 2;
                                break;
                            default:
                                collision |= 2;
                                break;
                        }
                    }
                }
                _context.Observation[Locals.OBSERVATION_COLLISION_INDEX] = collision;
               
                // Get angle from goal
                _context.Observation[Locals.OBSERVATION_ANGLE_FROM_GOAL_INDEX] = angle;
                // Get angle velocity
                _context.Observation[Locals.OBSERVATION_ANGLE_VELOCITY_INDEX] = _context.Bot.Direction;
                // Get distance
                _context.Observation[Locals.OBSERVATION_DISTANCE_INDEX] = distance;

                // Update distance
                _context.AddDistanceWalked(_context.Bot.transform.position);
            }).WaitOne();

            // Check if bot reached checkpoint
            if (Vector3.Distance(_context.BotPosition, _context.GoalPosition) < 5)
            {
                _context.ObservationsAroundGoal++;
            }

            if (_context.ObservationsAroundGoal >= 20)
            {
                _context.IsCheckpointReached = true;
            }
        }

        private void SetAction()
        {
            if (_context.IsTrainingTheNetwork)
            {
                _storeExperience = StoreExperience;
            }
            else
            {
                _storeExperience = Pass;
            }
        }

        private void StoreExperience()
        {
            // Remember
            _context.ExperienceBuffer.Enqueue(new ExperienceContainer(_context.InitialObservation, _context.Action, _context.Observation, CalculateReward(), _context.IsDead || _context.IsCheckpointReached || _context.IsTrapped));
        }

        private void FirstObserve()
        {
            SetAction();
        }

        private void Pass()
        {
        }

        #region Reward calculation

        private double CalculateReward()
        {
            double reward = 0;

            reward += CalculateRewardBasedOnAngleAndDistance(_context.Observation[Locals.OBSERVATION_ANGLE_FROM_GOAL_INDEX],
                _context.Observation[Locals.OBSERVATION_DISTANCE_INDEX]);
            //reward += CalculateRewardBasedOnDistance(_context.Observation[Locals.OBSERVATION_DISTANCE_INDEX]);
            reward += CalculateRewardBasedOnObstacle(_context.Observation[Locals.OBSERVATION_COLLISION_INDEX]);
            reward += CalculateRewardForDead(_context.IsDead);
            reward += CalculateRewardForTrapped(_context.IsTrapped);
            reward += CalculateRewardForCheckpoint(_context.IsCheckpointReached);
            reward += CalculateRewardForCollected();

            return reward;
        }

        private double CalculateRewardBasedOnAngleAndDistance(double angle, double distance)
        {
            return -1 * distance * Math.Abs(angle);
        }

        private double CalculateRewardBasedOnObstacle(double value)
        {
            switch (value)
            {
                case 2:
                case 3:
                    return -300;
                case 1:
                    return -150;
                default:
                    return 0;
            }
        }

        private double CalculateRewardForDead(bool value)
        {
            return value ? -10000 : 0;
        }

        private double CalculateRewardForTrapped(bool value)
        {
            return value ? -5000 : 0;
        }

        private double CalculateRewardForCheckpoint(bool value)
        {
            return value ? 10000 : 0;
        }

        private double CalculateRewardForCollected()
        {
            var result = _lastCollectedCount != _context.CollectedCount ? _context.CollectedCount : 0;
            _lastCollectedCount = _context.CollectedCount;
            return result;
        }

        #endregion

        #endregion
    }
}
