using Assets.Scripts.Prefabs.Bot.States.Context;

namespace Assets.Scripts.Prefabs.Bot.States
{
    public enum BotStateType
    {
        START,
        IDLE,
        OBSERVE,
        DEAD,
        CHECKPOINT,
        TRAPPED,
        PREDICT,
        REWARD,
        EXIT
    }

    class BotStatesFactory
    {
        public static BotState MakeBotState(BotStateType stateType, BotContext context)
        {
            BotState state = null;

            switch(stateType)
            {
                case BotStateType.START:
                    state = new Start(context);
                    break;
                case BotStateType.IDLE:
                    state = new Idle(context);
                    break;
                case BotStateType.OBSERVE:
                    state = new Observe(context);
                    break;
                case BotStateType.CHECKPOINT:
                    state = new Checkpoint(context);
                    break;
                case BotStateType.TRAPPED:
                    state = new Trapped(context);
                    break;
                case BotStateType.DEAD:
                    state = new Dead(context);
                    break;
                case BotStateType.PREDICT:
                    state = new Predict(context);
                    break;
                case BotStateType.REWARD:
                    state = new Reward(context);
                    break;
                case BotStateType.EXIT:
                    state = new Exit(context);
                    break;
            }

            return state;
        }
    }
}
