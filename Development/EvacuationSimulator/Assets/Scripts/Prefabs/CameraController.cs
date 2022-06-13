using Assets.Scripts.Prefabs.Bot.Observers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Prefabs
{
    class CameraController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI cameraUI;

        private int _currentCameraIndex;
        private GameObject _mainCamera;
        private readonly List<GameObject> _cameras = new List<GameObject>();
        private readonly object _lock = new object();

        private void Start()
        {
            _mainCamera = GameObject.Find("Main Camera");
            _cameras.Add(_mainCamera);
            _currentCameraIndex = 0;
            cameraUI.text = "Main";
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                lock (_lock)
                {
                    Switch();
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                lock (_lock)
                {
                    SwitchToMain();
                }
            }
        }

        public void Register(GameObject camera)
        {
            lock (_lock)
            {
                _cameras.Add(camera);
            }
        }

        public void Unregister(GameObject camera)
        {
            lock (_lock)
            {
                if (_cameras[_currentCameraIndex] == camera)
                {
                    Switch();
                    if (_currentCameraIndex != 0)
                    {
                        _currentCameraIndex--;
                    }
                }
                _cameras.Remove(camera);
            }
        }

        public void CheckSwitch(GameObject camera)
        {
            lock (_lock)
            {
                if (_cameras[_currentCameraIndex] == camera)
                {
                    Switch();
                }
            }
        }

        private void Switch()
        {
            var lastCameraIndex = _currentCameraIndex;

            do
            {
                _currentCameraIndex++;
                if (_currentCameraIndex >= _cameras.Count)
                {
                    _currentCameraIndex = 0;
                }
            } while (_currentCameraIndex != 0 && _cameras[_currentCameraIndex].transform.parent.gameObject.activeInHierarchy == false);

            _cameras[_currentCameraIndex].SetActive(true);
            _cameras[lastCameraIndex].SetActive(false);

            if (_currentCameraIndex == 0)
            {
                cameraUI.text = "Main";
            }
            else
            {
                cameraUI.text = _cameras[_currentCameraIndex].transform.parent.name;
            }
        }

        private void SwitchToMain()
        {
            _cameras[0].SetActive(true);
            if (_currentCameraIndex != 0 && _currentCameraIndex < _cameras.Count)
            {
                _cameras[_currentCameraIndex].SetActive(false);
            }
            _currentCameraIndex = 0;
            cameraUI.text = "Main";
        }

    }
}
