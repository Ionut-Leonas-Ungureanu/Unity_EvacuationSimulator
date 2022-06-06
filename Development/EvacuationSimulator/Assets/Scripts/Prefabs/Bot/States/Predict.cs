using Assets.Scripts.Prefabs.Bot.States.Context;
using Assets.Scripts.Prefabs.Bot.States.Constants;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Predict : BotState
    {
        public Predict(BotContext context) : base(context)
        {
        }

        protected override void HandleState()
        {

            _context.Action = _context.Brain.GetAction(_context.Observation);

            // Set action
            _context.Bot.UpdateDirection(Locals.DIRECTION_VALUES[_context.Action]);

            // Wait 100 ms
            //Thread.Sleep(300);

        }

        protected override void SetNextState()
        {
            if (_context.IsTrainingTheNetwork)
            {
                _context.SetState(BotStateType.REWARD);
            }
            else
            {
                _context.SetState(BotStateType.OBSERVE);
            }
        }
    }
}
