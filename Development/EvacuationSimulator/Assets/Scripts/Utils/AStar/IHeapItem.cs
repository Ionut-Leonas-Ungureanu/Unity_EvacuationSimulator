using System;

namespace Assets.Scripts.Utils.AStar
{
    interface IHeapItem<T> : IComparable<IHeapItem<T>>
    {
        T Data { get; set; }
        uint Index { get; set; }
    }
}
