using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils.AStar
{
    /// <summary>
    /// Min Heap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Heap<T> where T : IComparable<T>
    {
        private readonly IHeapItem<T>[] _items;
        private readonly HashSet<T> _set;
        private uint _count = 0;

        public uint Count => _count;

        public Heap(uint maxHeapSize)
        {
            _items = new NodeHeapItem<T>[maxHeapSize];
            _set = new HashSet<T>();
        }

        public void Add(T item)
        {
            var newItem = new NodeHeapItem<T>
            {
                Index = _count,
                Data = item
            };
            _set.Add(item);
            _items[_count] = newItem;
            SortUp(newItem);
            _count++;
        }

        public T RemoveFirst()
        {
            var firstItem = _items[0];
            _set.Remove(firstItem.Data);
            _count--;
            _items[0] = _items[_count];
            _items[0].Index = 0;
            SortDown(_items[0]);
            return firstItem.Data;
        }

        public void UpdateItem(IHeapItem<T> item)
        {
            SortUp(item);
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        private void SortDown(IHeapItem<T> item)
        {
            while (true)
            {
                uint childLeftIndex = item.Index * 2 + 1;
                uint childRightIndex = item.Index * 2 + 2;
                uint swapIndex = 0;

                if(childLeftIndex < _count)
                {
                    swapIndex = childLeftIndex;

                    if(childRightIndex < _count)
                    {
                        if(_items[childLeftIndex].CompareTo(_items[childRightIndex]) > 0)
                        {
                            swapIndex = childRightIndex;
                        }
                    }

                    if (item.CompareTo(_items[swapIndex]) > 0)
                    {
                        Swap(item, _items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SortUp(IHeapItem<T> item)
        {
            uint parentIndex;

            while (item.Index > 0)
            {
                parentIndex = (item.Index - 1) / 2;

                if (item.CompareTo(_items[parentIndex]) < 0)
                {
                    Swap(item, _items[parentIndex]);
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(IHeapItem<T> item1, IHeapItem<T> item2)
        {
            _items[item1.Index] = item2;
            _items[item2.Index] = item1;
            uint tempIndex = item1.Index;
            item1.Index = item2.Index;
            item2.Index = tempIndex;
        }
    }
}
