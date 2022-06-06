using Assets.Scripts.Prefabs.Bot.States.Context;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Bot.States
{
    abstract class FinishState : BotState
    {
        protected FinishState(BotContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            _context.Bot.StopMoving();
            _context.StopMeasuringEscapingTime();
            _context.HasToEscape = false;
            _context.PathConsumer.Stop();

            // Save neural network
            if (_context.IsTrainingTheNetwork)
            {
                _context.SaveNeuralNetwork();
            }

            _context.Bot.Notify();  // Notify subscribers
        }
        protected override void SetNextState()
        {
            _context.SetState(BotStateType.IDLE);
        }
    }
}
