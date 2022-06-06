using System;

namespace Assets.Scripts.Prefabs.Bot.Observers
{
    interface IBotObserver
    {
        Guid Id { get; set; }

        void Update();
    }
}
