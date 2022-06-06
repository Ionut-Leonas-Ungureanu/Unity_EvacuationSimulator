using Assets.Scripts.Utils.Results;
using System.Collections.Generic;

namespace Assets.Scripts.DesignPatterns.BotObserver
{
    internal class RunNumberNotifier
    {
        private List<IBotObserver> _observers;

        public RunResultsContainer ResultsContainer;

        public RunNumberNotifier()
        {
            _observers = new List<IBotObserver>();
        }

        public void Subscribe(IBotObserver newObserver)
        {
            _observers.Add(newObserver);
        }

        public void Unsubscribe(IBotObserver observer)
        {
            _observers.Remove(observer);
        }

        public void UnsubscribeAll()
        {
            _observers.Clear();
        }

        public void Notify()
        {
            foreach(var observer in _observers)
            {
                observer.UpdateObserver(ResultsContainer);
            }
        }
    }
}
