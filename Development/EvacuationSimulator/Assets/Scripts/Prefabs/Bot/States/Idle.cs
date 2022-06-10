using Assets.Scripts.Prefabs.Bot.States.Context;
using Assets.Scripts.Utils.AStar;
using Assets.Scripts.Utils.Processor;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Idle : BotState
    {
        public Idle(BotContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            lock (_context.Lock)
            {
                Monitor.Wait(_context.Lock);
            }

            if (_context.HasToEscape)
            {
                // Reset
                _context.Reset();

                var botPosition = _context.BotPosition;

                Thread.Sleep(500);

                // Set last bot position
                _context.LastBotPosition = botPosition;

                // Find Path
                _context.FindPath(botPosition);

                // Start To consume from path
                _context.PathConsumer = new Processor(_context.ConsumeNavigationPath);
                _context.PathConsumer.Start();

                // Start walking
                _context.StartMeasuringEscapingTime();
                _context.Bot.StartMoving();

                _context.ObservationsAroundGoal = 0;
            }
        }

        protected override void SetNextState()
        {
            if (_context.Terminate)
            {
                _context.SetState(BotStateType.EXIT);
                return;
            }

            if(_context.NavigationPath == null || _context.NavigationPath.Count == 0)
            {
                _context.SetState(BotStateType.TRAPPED);
                _context.IsTrapped = true;
                return;
            }

            _context.SetState(BotStateType.OBSERVE);
        }
    }
}
