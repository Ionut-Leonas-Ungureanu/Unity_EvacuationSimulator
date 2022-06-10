using Assets.Scripts.DesignPatterns.Singleton;
using System;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Assets.Scripts.Utils
{
    internal class BoxFireManager
    {
        private Vector3 _startCorner;
        private readonly float _length;
        private readonly float _width;
        private readonly float _height;
        private readonly float _rotation;

        private GameObject[,,] _boxOfInstances;
        private readonly float _prefabSize;
        private readonly float _scaledPrefabSize;
        private readonly float _scaling;
        private readonly Vector3 _scalingVector;

        private readonly int _numberObjectsAxisX;
        private readonly int _numberObjectsAxisY;
        private readonly int _numberObjectsAxisZ;

        private System.Random RandomFireSpreadPosition = new System.Random();

        private readonly Func<Vector3, GameObject> _createFireNodeCallback;
        private readonly Action<GameObject> _destroyCallback;

        public bool IsInitialized { get; set; }
        public bool IsSpreadingFire { get; set; }
        public bool IsReseting { get; set; }

        public Action OnInitialized { get; set; }

        public BoxFireManager(Func<Vector3, GameObject> createFireNodeCallback, Action<GameObject> destroyCallback, Vector3 startCorner, float length, float width, float height, float rotation, float prefabSize, float scaling = 1)
        {
            _startCorner = startCorner;
            _length = length;
            _width = width;
            _height = height;
            _rotation = rotation;
            _prefabSize = prefabSize;
            _scaling = scaling;
            _createFireNodeCallback = createFireNodeCallback;
            _destroyCallback = destroyCallback;
            _scalingVector = new Vector3(_scaling, _scaling, _scaling);

            _scaledPrefabSize = _prefabSize * _scaling;
            _numberObjectsAxisX = (int)Math.Floor(_width / _scaledPrefabSize);
            _numberObjectsAxisZ = (int)Math.Floor(_length / _scaledPrefabSize);
            _numberObjectsAxisY = (int)Math.Floor(_height / _scaledPrefabSize);
        }

        public IEnumerator Initialize()
        {
            if (IsInitialized)
                yield break;

            Quaternion orientation = Quaternion.AngleAxis(_rotation, Vector3.up);

            _boxOfInstances = new GameObject[_numberObjectsAxisX, _numberObjectsAxisY, _numberObjectsAxisZ];

            GameObject newFireInstance;
            for (var x = 0; x < _numberObjectsAxisX; ++x)
            {
                for (var y = 0; y < _numberObjectsAxisY; ++y)
                {
                    for (var z = 0; z < _numberObjectsAxisZ; ++z)
                    {
                        newFireInstance = _createFireNodeCallback(Vector3.zero);
                        newFireInstance.transform.Rotate(-90, _rotation, 0);
                        newFireInstance.transform.localScale = _scalingVector;
                        newFireInstance.SetActive(false);
                        _boxOfInstances[x, y, z] = newFireInstance;
                    }
                }
            }

            var xx = _boxOfInstances[0,0,0].transform.localScale;

            // Set the first position
            Vector3 basePositionAxisX = _startCorner + orientation * Vector3.forward * (_scaledPrefabSize / 2);
            basePositionAxisX += orientation * Vector3.left * (_scaledPrefabSize / 2);
            //basePositionAxisX += orientation * Vector3.up * (_scaledPrefabSize / 2);

            var vectors = new NativeArray<Vector3>(_numberObjectsAxisX * _numberObjectsAxisY * _numberObjectsAxisZ, Allocator.Persistent);

            var instantiateNodesJob = new InstantiateFireNodeJob
            {
                basePositionAxisX = basePositionAxisX,
                scaledPrefabSize = _scaledPrefabSize,
                orientation = orientation,
                xSize = _numberObjectsAxisX,
                ySize = _numberObjectsAxisY,
                zSize = _numberObjectsAxisZ,
                vectors = vectors
            };
            var xJobHandle = instantiateNodesJob.Schedule(_numberObjectsAxisX * _numberObjectsAxisY * _numberObjectsAxisZ, 1);
            xJobHandle.Complete();

            for (var x = 0; x < _numberObjectsAxisX; ++x)
            {
                for (var y = 0; y < _numberObjectsAxisY; ++y)
                {
                    for (var z = 0; z < _numberObjectsAxisZ; ++z)
                    {
                        var index = x * _numberObjectsAxisY * _numberObjectsAxisZ + y * _numberObjectsAxisZ + z;
                        var colliderSize = _boxOfInstances[x, y, z].GetComponent<BoxCollider>().size;
                        var center = vectors[index] + new Vector3(0, colliderSize.z/2, 0);
                        if (Physics.CheckBox(center, _boxOfInstances[x, y, z].transform.localScale / 2 - new Vector3(0.1f, 0.1f, 0.1f), orientation, -1, QueryTriggerInteraction.Collide))
                        {
                            _boxOfInstances[x, y, z].transform.position = vectors[index];
                        }
                        else
                        {
                            _destroyCallback(_boxOfInstances[x, y, z]);
                        }
                    }
                }
            }

            vectors.Dispose();

            //foreach (var fireNode in _boxOfInstances)
            //{
            //    if ((object)fireNode != null)
            //    {
            //        fireNode.SetActive(true);
            //    }
            //}

            IsInitialized = true;
            OnInitialized?.Invoke();
        }

        public IEnumerator SpreadFire()
        {
            if (IsSpreadingFire)
            {
                yield break;
            }

            IsSpreadingFire = true;

            int xSpawn, ySpawn, zSpawn;
            do
            {
                xSpawn = RandomFireSpreadPosition.Next(0, _numberObjectsAxisX*1000);
                ySpawn = RandomFireSpreadPosition.Next(0, _numberObjectsAxisY*1000);
                zSpawn = RandomFireSpreadPosition.Next(0, _numberObjectsAxisZ*1000);
                xSpawn /= 1000;
                ySpawn /= 1000;
                zSpawn /= 1000;
                yield return null;
            }
            while (xSpawn < 0 || xSpawn >= _numberObjectsAxisX ||
            ySpawn < 0 || ySpawn >= _numberObjectsAxisY ||
            zSpawn < 0 || zSpawn >= _numberObjectsAxisZ ||
            _boxOfInstances[xSpawn, ySpawn, zSpawn] == null);

            _boxOfInstances[xSpawn, ySpawn, zSpawn].SetActive(true);

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

                if (maxX < _numberObjectsAxisX - 1)
                {
                    maxX++;
                }

                if (minY > 0)
                {
                    minY--;
                }

                if (maxY < _numberObjectsAxisY - 1)
                {
                    maxY++;
                }

                if (minZ > 0)
                {
                    minZ--;
                }

                if (maxZ < _numberObjectsAxisZ - 1)
                {
                    maxZ++;
                }

                // back
                for (var x = minX; x <= maxX; ++x)
                {
                    for (var y = minY; y <= maxY; ++y)
                    {
                        if (_boxOfInstances[x, y, minZ] != null)
                        {
                            _boxOfInstances[x, y, minZ].SetActive(true);
                        }
                        yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed);
                    }
                }

                // front
                for (var x = minX; x <= maxX; ++x)
                {
                    for (var y = minY; y <= maxY; ++y)
                    {
                        if (_boxOfInstances[x, y, maxZ] != null)
                        {
                            _boxOfInstances[x, y, maxZ].SetActive(true);
                        }
                        yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed); ;
                    }
                }

                // right
                for (var z = minZ + 1; z < maxZ; ++z)
                {
                    for (var y = minY; y <= maxY; ++y)
                    {
                        if (_boxOfInstances[minX, y, z] != null)
                        {
                            _boxOfInstances[minX, y, z].SetActive(true);
                        }
                        yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed); ;
                    }
                }

                // left
                for (var z = minZ + 1; z < maxZ; ++z)
                {
                    for (var y = minY; y <= maxY; ++y)
                    {
                        if (_boxOfInstances[maxX, y, z] != null)
                        {
                            _boxOfInstances[maxX, y, z].SetActive(true);
                        }
                        yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed); ;
                    }
                }

                // down
                for (var x = minX + 1; x < maxX; ++x)
                {
                    for (var z = minZ + 1; z < maxZ; ++z)
                    {
                        if (_boxOfInstances[x, minY, z] != null)
                        {
                            _boxOfInstances[x, minY, z].SetActive(true);
                        }
                        yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed); ;
                    }
                }

                // up
                for (var x = minX + 1; x < maxX; ++x)
                {
                    for (var z = minZ + 1; z < maxZ; ++z)
                    {
                        if (_boxOfInstances[x, maxY, z] != null)
                        {
                            _boxOfInstances[x, maxY, z].SetActive(true);
                        }
                        yield return new WaitForSeconds(SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed); ;
                    }
                }

            } while (minX > 0 || minY > 0 || minZ > 0
                    || maxX < _numberObjectsAxisX - 1
                    || maxY < _numberObjectsAxisY - 1
                    || maxZ < _numberObjectsAxisZ - 1);

            IsSpreadingFire = false;

        }

        public void Reset()
        {
            IsReseting = true;

            foreach (var fireNode in _boxOfInstances)
            {
                if (fireNode != null)
                {
                    fireNode.SetActive(false);
                }
            }

            IsReseting = false;
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
        public float scaledPrefabSize;
        [ReadOnly]
        public Quaternion orientation;
        [ReadOnly]
        public Vector3 basePositionAxisX;

        public NativeArray<Vector3> vectors;


        public void Execute(int index)
        {
            // Get idx, idy, idz
            var idx = index / (zSize * ySize);
            var idy = index % (zSize * ySize) / zSize;
            var idz = index % (zSize * ySize) % zSize;

            var position = basePositionAxisX + orientation * Vector3.left * scaledPrefabSize * idx;
            position += orientation * Vector3.up * scaledPrefabSize * idy;
            position += orientation * Vector3.forward * scaledPrefabSize * idz;

            vectors[index] = position;
        }
    }
}
