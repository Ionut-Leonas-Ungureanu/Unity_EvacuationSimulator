using Assets.Scripts.Prefabs.Bot.States.Context;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Start : Idle
    {
        public Start(BotContext context) : base(context)
        {
        }

        protected override void SetNextState()
        {
            base.SetNextState();

            if (_context.IsTrainingTheNetwork)
            {
                // if it is training
            }
        }
    }
}
