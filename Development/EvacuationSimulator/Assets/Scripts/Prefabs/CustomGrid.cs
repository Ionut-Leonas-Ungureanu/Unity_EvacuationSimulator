using Assets.Scripts.Utils.AStar;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    public Vector3 gridWorldSize;
    //public GameObject start;
    //public Vector3 target;
    public float nodeRadius;

    public Action OnGridGenerated { get; set; }

    private void Start()
    {
        Navigator.NodeDiameter = nodeRadius * 2;
        Navigator.GridSizeX = Mathf.RoundToInt(gridWorldSize.x / Navigator.NodeDiameter);
        Navigator.GridSizeY = Mathf.RoundToInt(gridWorldSize.y / Navigator.NodeDiameter);
        Navigator.GridSizeZ = Mathf.RoundToInt(gridWorldSize.z / Navigator.NodeDiameter);
    }

    public IEnumerator Generate()
    {
        Navigator.GridWorldSize = gridWorldSize;
        Navigator.Grid = new Node[Navigator.GridSizeX, Navigator.GridSizeY, Navigator.GridSizeZ];
        Navigator.WorldBottomLeft = transform.position + Vector3.left * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2 - Vector3.forward * gridWorldSize.z / 2;

        var positions = new NativeArray<Vector3>(Navigator.GridSizeX * Navigator.GridSizeY * Navigator.GridSizeZ, Allocator.Persistent);

        var instantiateNodesJob = new InstantiateGridNodeJob
        {
            xSize = Navigator.GridSizeX,
            ySize = Navigator.GridSizeY,
            zSize = Navigator.GridSizeZ,
            nodeDiameter = Navigator.NodeDiameter,
            nodeRadius = nodeRadius,
            basePosition = Navigator.WorldBottomLeft,
            vectors = positions
        };
        var xJobHandle = instantiateNodesJob.Schedule(Navigator.GridSizeX * Navigator.GridSizeY * Navigator.GridSizeZ, 1);
        xJobHandle.Complete();

        // Create
        for (var x = 0; x < Navigator.GridSizeX; x++)
        {
            for (var y = 0; y < Navigator.GridSizeY; y++)
            {
                for (var z = 0; z < Navigator.GridSizeZ; z++)
                {
                    var positionIndex = x * Navigator.GridSizeY * Navigator.GridSizeZ + y * Navigator.GridSizeZ + z;
                    Node node = new Node();
                    node.Position = new Vector3(positions[positionIndex].x, positions[positionIndex].y, positions[positionIndex].z);
                    
                    var colliders = Physics.OverlapBox(node.Position, Vector3.one * nodeRadius);
                    //if (!Physics.CheckSphere(node.Position, nodeRadius))
                    if (!colliders.Any(_ => _.name.Contains("Office building")
                        || _.name.Contains("Metal")
                        || _.name.Contains("Window")
                        || _.name.Contains("Terrain")
                        || _.name.Contains("Railing")
                        || _.name.Contains("reception")
                        || _.CompareTag("TrainingGround")))
                    {
                        Navigator.Grid[x, y, z] = node;
                        node.GridX = x;
                        node.GridY = y;
                        node.GridZ = z;

                        // Check again for props
                        if (Physics.CheckBox(node.Position, Vector3.one * nodeRadius))
                        {
                            node.Walkable = false;
                        }
                    }
                    else
                    {
                        Navigator.Grid[x, y, z] = null;
                    }
                }
            }
            yield return null;
        }

        positions.Dispose();

        new Thread(() =>
        {
            // Filter
            for (var x = 0; x < Navigator.GridSizeX; x++)
            {
                for (var y = 0; y < Navigator.GridSizeY; y++)
                {
                    for (var z = 0; z < Navigator.GridSizeZ; z++)
                    {
                        if (Navigator.Grid[x, y, z] != null)
                        {
                            if (y == Navigator.GridSizeY - 1)
                            {
                                Navigator.Grid[x, y, z].Walkable = false;
                            }

                            // if neightbour top and neighbour under
                            if (y > 0
                                && y < Navigator.GridSizeY - 1
                                && Navigator.Grid[x, y + 1, z] != null
                                && Navigator.Grid[x, y - 1, z] != null)
                            {
                                Navigator.Grid[x, y, z].Walkable = false;
                            }

                            //// if neighbour top but no neighbout under
                            if (y > 0
                                && y < Navigator.GridSizeY - 1
                                && Navigator.Grid[x, y - 1, z] != null
                                && Navigator.Grid[x, y + 1, z] == null)
                            {
                                Navigator.Grid[x, y, z].Walkable = false;
                            }

                            //if ((x > 0 && x < _gridSizeX - 1) && ((grid[x - 1, y, z] == null && grid[x + 1, y, z] == null) ))
                            //{
                            //    grid[x, y, z].Walkable = false;
                            //}

                            //if ((z > 0 && z < _gridSizeZ - 1) && ((grid[x, y, z - 1] == null && grid[x, y, z + 1] == null) ))
                            //{
                            //    grid[x, y, z].Walkable = false;
                            //}

                            //if((x > 0 && x < _gridSizeX - 1) && (z > 0 && z < _gridSizeZ - 1) && (grid[x - 1, y, z] == null && grid[x + 1, y, z] == null) &&
                            //    (grid[x, y, z - 1] == null && grid[x, y, z + 1] == null))
                            //{
                            //    grid[x, y, z].Walkable = false;
                            //}
                        }
                    }
                }
            }

            // Clear
            for (var x = 0; x < Navigator.GridSizeX; x++)
            {
                for (var y = 0; y < Navigator.GridSizeY; y++)
                {
                    for (var z = 0; z < Navigator.GridSizeZ; z++)
                    {
                        if (Navigator.Grid[x, y, z] != null && Navigator.Grid[x, y, z].Walkable == false)
                        {
                            Navigator.Grid[x, y, z] = null;
                        }
                    }
                }
            }

            Navigator.IsGenerated = true;
            OnGridGenerated?.Invoke();
        })
        {
            IsBackground = true
        }.Start();

        yield return null;
    }

    //public List<Node> Path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        //if (Grid != null && _isGenerated)
        //{
        //    foreach (var node in Grid)
        //    {
        //        if (node != null)
        //        {
        //            Gizmos.color = Color.red;
        //            if (Path != null)
        //            {
        //                Gizmos.color = Color.red;
        //                if (Path.Contains(node))
        //                {
        //                    Gizmos.color = Color.blue;
        //                    if (Path.IndexOf(node) == 0)
        //                    {
        //                        Gizmos.color = Color.green;
        //                    }
        //                }
        //            }
        //            Gizmos.DrawCube(node.Position, Vector3.one * (_nodeDiameter - 0.1f));

        //        }
        //    }
        //}


    }

    [BurstCompile]
    struct InstantiateGridNodeJob : IJobParallelFor
    {
        [ReadOnly]
        public int xSize;
        [ReadOnly]
        public int ySize;
        [ReadOnly]
        public int zSize;
        [ReadOnly]
        public float nodeDiameter;
        [ReadOnly]
        public float nodeRadius;
        [ReadOnly]
        public Vector3 basePosition;

        public NativeArray<Vector3> vectors;


        public void Execute(int index)
        {
            // Get idx, idy, idz
            var idx = index / (zSize * ySize);
            var idy = index % (zSize * ySize) / zSize;
            var idz = index % (zSize * ySize) % zSize;

            var position = basePosition;
            position -= Vector3.left * (idx * nodeDiameter + nodeRadius);
            position += Vector3.up * (idy * nodeDiameter + nodeRadius);
            position += Vector3.forward * (idz * nodeDiameter + nodeRadius);

            vectors[index] = position;
        }
    }
}
