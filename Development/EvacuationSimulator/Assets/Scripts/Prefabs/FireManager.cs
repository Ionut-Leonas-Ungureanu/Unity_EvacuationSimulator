using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Utils.AStar;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public Vector3 gridSize;
    public GameObject firePrefab;

    private int _sizeX;
    private int _sizeY;
    private int _sizeZ;
    private float _nodeDiameter;
    private float _nodeRadius;
    private Vector3 _worldBottomLeft;
    private bool _isGenerated;
    private bool _isSpreadingFire;
    private System.Random RandomFireSpreadPosition;

    public GameObject[,,] FireNodes;
    public bool IsGenerated => _isGenerated;
    public bool IsSpreadingFire => _isSpreadingFire;
    public delegate void Execute();
    public event Execute OnGenerated;
    public event Execute OnFireNodesDisabled;

    public IEnumerator Generate()
    {
        if (IsGenerated)
        {
            yield break;
        }

        _nodeDiameter = SimulationConfigurator.Instance.FireSettings.FireNodeScale;
        _nodeRadius = _nodeDiameter / 2;
        _sizeX = Mathf.RoundToInt(gridSize.x / _nodeDiameter);
        _sizeY = Mathf.RoundToInt(gridSize.y / _nodeDiameter);
        _sizeZ = Mathf.RoundToInt(gridSize.z / _nodeDiameter);
        FireNodes = new GameObject[_sizeX, _sizeY, _sizeZ];

        _worldBottomLeft = transform.position + Vector3.left * gridSize.x / 2 - Vector3.up * gridSize.y / 2 - Vector3.forward * gridSize.z / 2;

        var positions = new NativeArray<Vector3>(_sizeX * _sizeY * _sizeZ, Allocator.Persistent);

        var instantiateNodesJob = new InstantiateFireNodeJob
        {
            xSize = _sizeX,
            ySize = _sizeY,
            zSize = _sizeZ,
            nodeDiameter = _nodeDiameter,
            nodeRadius = _nodeRadius,
            basePosition = _worldBottomLeft,
            vectors = positions
        };
        var xJobHandle = instantiateNodesJob.Schedule(_sizeX * _sizeY * _sizeZ, 1);
        xJobHandle.Complete();

        // Create gameObjects
        for (var x = 0; x < _sizeX; ++x)
        {
            for (var y = 0; y < _sizeY; ++y)
            {
                for (var z = 0; z < _sizeZ; ++z)
                {
                    var positionIndex = x * _sizeY * _sizeZ + y * _sizeZ + z;
                    if (Physics.OverlapBox(positions[positionIndex], Vector3.one * _nodeRadius).Any())
                    {
                        FireNodes[x, y, z] = Instantiate(firePrefab, positions[positionIndex], Quaternion.identity);
                        FireNodes[x, y, z].transform.localScale = Vector3.one * _nodeDiameter;
                        FireNodes[x, y, z].transform.Rotate(-90, 0, 0);
                        FireNodes[x, y, z].transform.parent = transform;
                        FireNodes[x, y, z].SetActive(false);
                    }
                    else
                    {
                        FireNodes[x, y, z] = null;
                    }
                }
            }
        }

        positions.Dispose();

        _isGenerated = true;
        OnGenerated?.Invoke();

        yield return null;
    }

    public IEnumerator SpreadFire()
    {
        if (IsSpreadingFire)
        {
            yield break;
        }

        _isSpreadingFire = true;
        RandomFireSpreadPosition = new System.Random();

        int xSpawn, ySpawn, zSpawn;
        do
        {
            xSpawn = RandomFireSpreadPosition.Next(0, _sizeX);
            ySpawn = RandomFireSpreadPosition.Next(0, _sizeY);
            zSpawn = RandomFireSpreadPosition.Next(0, _sizeZ);
        }
        while (xSpawn < 0 || xSpawn >= _sizeX ||
        ySpawn < 0 || ySpawn >= _sizeY ||
        zSpawn < 0 || zSpawn >= _sizeZ ||
        FireNodes[xSpawn, ySpawn, zSpawn] == null);

        ActivateFireNode(xSpawn, ySpawn, zSpawn);
        yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);

        var minX = xSpawn;
        var maxX = xSpawn;
        var minY = ySpawn;
        var maxY = ySpawn;
        var minZ = zSpawn;
        var maxZ = zSpawn;

        do
        {

            if (minX > 0)
            {
                minX--;
            }

            if (maxX < _sizeX - 1)
            {
                maxX++;
            }

            if (minY > 0)
            {
                minY--;
            }

            if (maxY < _sizeY - 1)
            {
                maxY++;
            }

            if (minZ > 0)
            {
                minZ--;
            }

            if (maxZ < _sizeZ - 1)
            {
                maxZ++;
            }

            // back
            for (var x = minX; x <= maxX; ++x)
            {
                for (var y = minY; y <= maxY; ++y)
                {
                    ActivateFireNode(x, y, minZ);
                    yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);
                }
            }

            // front
            for (var x = minX; x <= maxX; ++x)
            {
                for (var y = minY; y <= maxY; ++y)
                {
                    ActivateFireNode(x, y, maxZ);
                    yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);
                }
            }

            // right
            for (var z = minZ; z < maxZ; ++z)
            {
                for (var y = minY; y <= maxY; ++y)
                {
                    ActivateFireNode(minX, y, z);
                    yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);
                }
            }

            // left
            for (var z = minZ; z < maxZ; ++z)
            {
                for (var y = minY; y <= maxY; ++y)
                {
                    ActivateFireNode(maxX, y, z);
                    yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);
                }
            }

            // down
            for (var x = minX; x < maxX; ++x)
            {
                for (var z = minZ; z < maxZ; ++z)
                {
                    ActivateFireNode(x, minY, z);
                    yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);
                }
            }

            // up
            for (var x = minX; x < maxX; ++x)
            {
                for (var z = minZ; z < maxZ; ++z)
                {
                    ActivateFireNode(x, maxY, z);
                    yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);
                }
            }

        } while (minX > 0 || minY > 0 || minZ > 0
                || maxX < _sizeX - 1
                || maxY < _sizeY - 1
                || maxZ < _sizeZ - 1);

        _isSpreadingFire = false;

    }

    public IEnumerator DisableFireNodes()
    {
        foreach(var node in FireNodes)
        {
            if (node != null)
            {
                node.SetActive(false);
            }
        }

        _isSpreadingFire = false;

        OnFireNodesDisabled?.Invoke();
        yield return null;
    }

    private void ActivateFireNode(int x, int y, int z)
    {
            if (FireNodes[x, y, z] != null)
            {
                var position = FireNodes[x, y, z].transform.position;
                Navigator.SetNodesUnwalkable(position, _nodeRadius);
                FireNodes[x, y, z].SetActive(true);
            }
    }

    [BurstCompile]
    struct InstantiateFireNodeJob : IJobParallelFor
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
