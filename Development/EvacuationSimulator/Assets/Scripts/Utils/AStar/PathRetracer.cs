using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils.AStar
{
    class PathRetracer
    {
        private Dictionary<Node, Node> _items = new Dictionary<Node, Node>();

        public void Add(Node node, Node parent)
        {
            if (_items.ContainsKey(node))
            {
                _items[node] = parent;
            }
            else
            {
                _items.Add(node, parent);
            }
        }

        public List<Node> RetraceBack(Node from, Node to)
        {
            var path = new List<Node>();
            Node current = from;

            while (_items.ContainsKey(current))
            {
                path.Add(current);
                current = _items[current];
            }

            path.Reverse();
            return path;
        }
    }
}
