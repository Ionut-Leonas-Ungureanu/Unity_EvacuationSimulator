using System;

namespace Assets.Scripts.Prefabs.Bot.Observers
{
    class CustomizedBotObserver : IBotObserver
    {
        public Guid Id { get; set; }
        public Action _action;

        public CustomizedBotObserver(Action action)
        {
            Id = Guid.NewGuid();
            _action = action;
        }

        public void Update()
        {
            _action?.Invoke();
        }
    }
}
