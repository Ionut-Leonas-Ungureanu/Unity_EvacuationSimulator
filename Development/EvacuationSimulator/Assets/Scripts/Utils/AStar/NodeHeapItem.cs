using System;

namespace Assets.Scripts.Utils.AStar
{
    class NodeHeapItem<T> : IHeapItem<T> where T : IComparable<T>
    {
        public T Data { get; set; }
        public uint Index { get; set; }

        public int CompareTo(IHeapItem<T> other)
        {
            return Data.CompareTo(other.Data);
        }
    }
}
