using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Prefabs.Bot;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    class BotsManager
    {
        public BotController[] Bots;
        private Vector3 _centerPosition;
        private float _radius;
        private Func<GameObject, Vector3, GameObject> _instantiateCallback;
        private GameObject _prefab;
        private GameObject[] _spawnPositions;
        private bool[] _spawnPositionTaken;
        private System.Random RandomBotPosition = new System.Random();

        public bool IsInitialized { get; set; }
        public bool IsReseting { get; set; }

        public Action OnInitialized { get; set; }

        public BotsManager(GameObject prefab, Func<GameObject, Vector3, GameObject> instantiateCallback, uint numberOfBots)
        {
            Bots = new BotController[numberOfBots];
            _prefab = prefab;
            _instantiateCallback = instantiateCallback;
        }

        public IEnumerator Generate()
        {
            if (IsInitialized)
                yield break;

            _spawnPositions = GameObject.FindGameObjectsWithTag("SpawnPosition");
            _spawnPositionTaken = new bool[_spawnPositions.Length];

            var mainCamera = GameObject.Find("Main Camera"); ;
            
            for (var i = Bots.Length - 1; i >= 0; --i)
            {
                // Generate bot
                var position = GenerateBotPosition();
                //var position = new Vector3(-20, 0.12f, 200);
                var bot = _instantiateCallback(_prefab, position);
                bot.name = $"{_prefab.name} {i}";

                // Configure bot
                var botController = bot.GetComponent<BotController>();
                botController.Identifier = i;

                // Hold instance
                Bots[i] = botController;

                yield return null;
            }

            IsInitialized = true;
            OnInitialized?.Invoke();
        }

        public void Reset()
        {
            IsReseting = true;

            _spawnPositions = GameObject.FindGameObjectsWithTag("SpawnPosition");
            _spawnPositionTaken = new bool[_spawnPositions.Length];

            for (int i = 0; i< _spawnPositionTaken.Length; ++i)
            {
                _spawnPositionTaken[i] = false;
            }

            for(int i=0; i<Bots.Length; ++i)
            {
                Bots[i].gameObject.SetActive(false);
                Bots[i].transform.position = GenerateBotPosition();
                Bots[i].gameObject.SetActive(true);
                Bots[i].Show();
            }

            IsReseting = false;
        }

        public bool IsAnyBotActive()
        {
            return Bots.Any(_ => _.IsActive);
        }

        private Vector3 GenerateBotPosition()
        {
            int index;
            do
            {
                index = RandomBotPosition.Next(_spawnPositions.Length);
            } while (_spawnPositionTaken[index]);
            _spawnPositionTaken[index] = true;
            
            return _spawnPositions[index].transform.position;
        }
    }
}
