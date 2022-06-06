using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils.AStar
{
    static class Navigator
    {
        public static bool IsGenerated { get; set; }
        public static Vector3 WorldBottomLeft { get; set; }
        public static Vector3 GridWorldSize { get; set; }
        public static Node[,,] Grid { get; set; }
        public static int GridSizeX { get; set; }
        public static int GridSizeY { get; set; }
        public static int GridSizeZ { get; set; }
        public static float NodeDiameter { get; set; }

        private static ReaderWriterLock _lock = new ReaderWriterLock();
        private static List<(int x, int y, int z)> _nodesInvalidated = new List<(int x, int y, int z)>();

        public static List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            if(Grid == null)
            {
                return null;
            }

            try
            {
                _lock.AcquireReaderLock(Timeout.Infinite);

                //Debug.Log("Navigator: Finding path");
                var tracer = new PathRetracer();
                var containers = new Dictionary<Node, AStarNodeContainer<Node>>();
                Node startNode = NodeFromWorldPoint(startPos);
                Node targetNode = NodeFromWorldPoint(targetPos);

                if(startNode == null)
                {
                    Debug.LogWarning($"StartNode {startPos} is null.");
                    return null;
                }

                if (targetNode == null)
                {
                    Debug.LogWarning("TargetNode is null.");
                    return null;
                }

                var openSet = new Heap<AStarNodeContainer<Node>>((uint)(GridSizeX * GridSizeY));
                var closedSet = new HashSet<Node>();

                var startContainer = new AStarNodeContainer<Node>(startNode);
                openSet.Add(startContainer);

                while (openSet.Count > 0)
                {
                    var currentNodeContainer = openSet.RemoveFirst();
                    var currentNode = currentNodeContainer.Data;
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        break;
                    }

                    foreach (Node neighbour in Neighbours(currentNode))
                    {
                        AStarNodeContainer<Node> neighbourContainer;
                        if (containers.ContainsKey(neighbour))
                        {
                            neighbourContainer = containers[neighbour];
                        }
                        else
                        {
                            neighbourContainer = new AStarNodeContainer<Node>(neighbour);
                            containers.Add(neighbour, neighbourContainer);
                        }

                        float cost = currentNodeContainer.G + Distance(currentNode, neighbour) * neighbour.GridY;

                        if (closedSet.Contains(neighbour) && neighbourContainer.G < cost)
                        {
                            continue;
                        }

                        if (cost < neighbourContainer.G || !openSet.Contains(neighbourContainer))
                        {
                            neighbourContainer.G = cost;
                            neighbourContainer.H = Distance(neighbour, targetNode) ;
                            tracer.Add(neighbour, currentNode);

                            if (!openSet.Contains(neighbourContainer))
                            {
                                openSet.Add(neighbourContainer);
                            }
                        }
                    }
                }

                var path = tracer.RetraceBack(targetNode, startNode);
                //Debug.Log($"Navigator: Returning path of length {path.Count}");
                return path;
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
            
        }

        private static float Distance(Node a, Node b)
        {
            return Mathf.Sqrt(Mathf.Pow((b.Position.x - a.Position.x), 2) + Mathf.Pow((b.Position.y - a.Position.y), 2) + Mathf.Pow((b.Position.z - a.Position.z), 2));
        }

        public static List<Node> Neighbours(Node node)
        {
            var result = new List<Node>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = 1; y >= -1; y--)
                {
                    for (var z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                        {
                            continue;
                        }

                        int checkX = node.GridX + x;
                        int checkY = node.GridY + y;
                        int checkZ = node.GridZ + z;

                        if (checkX >= 0
                            && checkX < GridSizeX
                            && checkY >= 0
                            && checkY < GridSizeY
                            && checkZ >= 0
                            && checkZ < GridSizeZ
                            && Grid[checkX, checkY, checkZ] != null
                            && Grid[checkX, checkY, checkZ].Walkable)
                        {
                            result.Add(Grid[checkX, checkY, checkZ]);
                        }
                    }
                }
            }

            return result;
        }

        public static List<Node> Neighbours(int node_x, int node_y, int node_z)
        {
            var result = new List<Node>();

            for (var x = -2; x <= 2; x++)
            {
                for (var y = 3; y >= 0; y--)
                {
                    for (var z = -2; z <= 2; z++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                        {
                            continue;
                        }

                        int checkX = node_x + x;
                        int checkY = node_y + y;
                        int checkZ = node_z + z;

                        if (checkX >= 0
                            && checkX < GridSizeX
                            && checkY >= 0
                            && checkY < GridSizeY
                            && checkZ >= 0
                            && checkZ < GridSizeZ
                            && Grid[checkX, checkY, checkZ] != null
                            && Grid[checkX, checkY, checkZ].Walkable)
                        {
                            result.Add(Grid[checkX, checkY, checkZ]);
                        }
                    }
                }
            }

            return result;
        }

        public static Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            (int x, int y, int z) = GetIndexInGrid(worldPosition);
           
            if (Grid[x, y, z] == null || !Grid[x, y, z].Walkable)
            {
                foreach (Node neighbour in Neighbours(x, y, z))
                {
                    return neighbour;
                }
            }

            return Grid[x, y, z];
        }

        public static void SetNodesUnwalkable(Vector3 position, float radius)
        {
            try
            {
                _lock.AcquireWriterLock(Timeout.Infinite);

                (int x, int y, int z) = GetIndexInGrid(position);

                var nodesPerRadius = Mathf.CeilToInt(radius / NodeDiameter + NodeDiameter);

                for (var idx = -nodesPerRadius; idx <= nodesPerRadius; ++idx)
                {
                    for (var idy = -nodesPerRadius; idy <= nodesPerRadius; ++idy)
                    {
                        for (var idz = -nodesPerRadius; idz <= nodesPerRadius; ++idz)
                        {
                            int checkX = x + idx;
                            int checkY = y + idy;
                            int checkZ = z + idz;

                            if (checkX >= 0
                                && checkX < GridSizeX
                                && checkY >= 0
                                && checkY < GridSizeY
                                && checkZ >= 0
                                && checkZ < GridSizeZ
                                && Grid[checkX, checkY, checkZ] != null)
                            {
                                Grid[checkX, checkY, checkZ].Walkable = false;
                                _nodesInvalidated.Add((checkX, checkY, checkZ));
                            }
                        }
                    }
                }
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        public static void ResetGrid()
        {
            try
            {
                _lock.AcquireWriterLock(Timeout.Infinite);

                foreach (var index in _nodesInvalidated)
                {
                    Grid[index.x, index.y, index.z].Walkable = true;
                }

                _nodesInvalidated = new List<(int x, int y, int z)>();
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        private static (int x, int y, int z) GetIndexInGrid(Vector3 position)
        {
            float percentX = Mathf.Abs(position.x - WorldBottomLeft.x) * 100 / GridWorldSize.x / 100;
            float percentY = Mathf.Abs(position.y - WorldBottomLeft.y) * 100 / GridWorldSize.y / 100;
            float percentZ = Mathf.Abs(position.z - WorldBottomLeft.z) * 100 / GridWorldSize.z / 100;

            int x = Mathf.RoundToInt((GridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((GridSizeY - 1) * percentY);
            int z = Mathf.RoundToInt((GridSizeZ - 1) * percentZ);

            return (x, y, z);
        }
    }
}
