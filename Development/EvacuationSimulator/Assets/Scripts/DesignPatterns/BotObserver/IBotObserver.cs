using Assets.Scripts.Utils.Results;

namespace Assets.Scripts.DesignPatterns.BotObserver
{
    internal interface IBotObserver
    {
        void UpdateObserver(RunResultsContainer container);
    }
}
