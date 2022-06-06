using Assets.Scripts.Prefabs.Bot.States.Context;

namespace Assets.Scripts.Prefabs.Bot.States
{
    abstract class BotState
    {
        #region Properties

        protected BotContext _context;

        #endregion

        public BotState(BotContext context)
        {
            _context = context;
        }

        public virtual void Handle()
        {
            // Execute current state
            HandleState();
            // Set which state is next
            SetNextState();
        }

        protected abstract void HandleState();
        protected abstract void SetNextState();
    }
}
