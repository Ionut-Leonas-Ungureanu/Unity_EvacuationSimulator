using System;

namespace Assets.Scripts.Prefabs.Bot.Observers
{
    class CustomizedBotObserver : IBotObserver
    {
        public Guid Id { get; set; }
        public Action<BotController> _action;

        public CustomizedBotObserver(Action<BotController> action)
        {
            Id = Guid.NewGuid();
            _action = action;
        }

        public void Update(BotController botController)
        {
            _action?.Invoke(botController);
        }
    }
}
