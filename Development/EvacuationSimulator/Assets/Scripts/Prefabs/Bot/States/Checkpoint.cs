using Assets.Scripts.Prefabs.Bot.States.Context;
using UnityEngine;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Checkpoint : FinishState
    {
        public Checkpoint(BotContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            _context.Status = BotStatus.SAFE;
            base.HandleState();

            Debug.Log("Checkpoint reached!");
        }
    }
}
