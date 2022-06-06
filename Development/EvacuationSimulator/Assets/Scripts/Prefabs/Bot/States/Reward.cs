using Assets.Scripts.Prefabs.Bot.States.Context;

namespace Assets.Scripts.Prefabs.Bot.States
{
    class Reward : BotState
    {

        public Reward(BotContext context) : base(context)
        {
        }

        protected override void HandleState()
        {
            if (_context.ExperienceBuffer.TryDequeue(out var experience))
            {
                // Rememeber
                _context.Brain.Remember(experience.Initial,
                    experience.Action,
                    experience.Final,
                    experience.Reward,
                    experience.IsOver);

                // Train
                _context.Brain.ReplayExperience();
            }
        }

        protected override void SetNextState()
        {
            _context.SetState(BotStateType.OBSERVE);
        }
    }
}
