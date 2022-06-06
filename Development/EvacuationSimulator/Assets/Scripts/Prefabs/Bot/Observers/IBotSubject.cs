using System;
using System.Collections.Generic;

namespace Assets.Scripts.Prefabs.Bot.Observers
{
    interface IBotSubject
    {
        Dictionary<Guid, IBotObserver> Observers { get; }

        void Subscribe(IBotObserver observer);
        void UnSubscribe(IBotObserver observer);
        void Notify();
    }
}
