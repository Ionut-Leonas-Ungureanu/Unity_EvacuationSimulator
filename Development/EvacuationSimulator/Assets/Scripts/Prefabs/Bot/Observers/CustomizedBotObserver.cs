using System;

namespace Assets.Scripts.Prefabs.Bot.Observers
{
    class CustomizedBotObserver : IBotObserver
    {
        public Guid Id { get; set; }
        public Action<object> _action;

        public CustomizedBotObserver(Action<object> action)
        {
            Id = Guid.NewGuid();
            _action = action;
        }

        public void Update(object args)
        {
            _action?.Invoke(args);
        }
    }
}
