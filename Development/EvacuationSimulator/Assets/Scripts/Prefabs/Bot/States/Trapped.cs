using Assets.Scripts.Prefabs.Bot.States.Context;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Trapped : FinishState
    {
        public Trapped(BotContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            _context.Status = BotStatus.TRAPPED;
            base.HandleState();

            _context.Bot.Dispatcher.Schedule(() =>
            {
                _context.Bot.Hide();
            }).WaitOne();
        }
    }
}
