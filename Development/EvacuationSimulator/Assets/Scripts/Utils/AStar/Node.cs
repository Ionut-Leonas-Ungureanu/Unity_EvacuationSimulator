using System;
using UnityEngine;

namespace Assets.Scripts.Utils.AStar
{
    public class Node
    {
        public Vector3 Position { get; set; }
        public bool Walkable { get; set; } = true;
        public int GridX { get; set; }
        public int GridY { get; set; }
        public int GridZ { get; set; }
    }
}
