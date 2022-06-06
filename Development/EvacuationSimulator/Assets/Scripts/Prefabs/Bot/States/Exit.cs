using Assets.Scripts.Prefabs.Bot.States.Context;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Exit : BotState
    {
        public Exit(BotContext context) : base(context)
        {
        }

        public override void Handle()
        {
            // Save network
            _context.SaveNeuralNetwork();

            // Terminate
        }

        protected override void HandleState()
        {
        }

        protected override void SetNextState()
        {
        }
    }
}
