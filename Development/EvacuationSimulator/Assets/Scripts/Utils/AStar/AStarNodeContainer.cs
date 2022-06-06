using System;

namespace Assets.Scripts.Utils.AStar
{
    class AStarNodeContainer<T> : IComparable<AStarNodeContainer<T>>
    {
        public T Data { get; set; }
        public float G { get; set; }
        public float H { get; set; }
        public float F => G + H;

        public AStarNodeContainer(T node)
        {
            Data = node;
        }

        public int CompareTo(AStarNodeContainer<T> element)
        {
            int compare = F.CompareTo(element.F);
            if (compare == 0)
            {
                compare = H.CompareTo(element.H);
            }
            return compare;
        }
    }
}
