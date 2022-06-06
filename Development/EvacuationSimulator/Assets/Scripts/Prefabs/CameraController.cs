using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Prefabs
{
    class CameraController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI cameraUI;

        private int _currentCameraIndex;
        private bool _isMainCameraActive;
        private GameObject _mainCamera;
        private List<GameObject> _cameras = new List<GameObject>();

        public void Register(GameObject camera)
        {
            _cameras.Add(camera);
        }

        public void Unregister(GameObject camera)
        {
            _cameras.Remove(camera);
            if(_cameras.Count == 0)
            {
                SwitchToMain();
            }
        }

        public void Switch()
        {
            if (_cameras.Count == 0)
            {
                return;
            }

            if (!_isMainCameraActive)
            {
                var nextCameraIndex = _currentCameraIndex + 1;
                if (nextCameraIndex >= _cameras.Count)
                {
                    SwitchToMain();
                    return;
                }
                else
                {
                    _cameras[nextCameraIndex].SetActive(true);
                    _cameras[_currentCameraIndex].SetActive(false);
                    _currentCameraIndex = nextCameraIndex;
                }
            }

            if (_isMainCameraActive)
            {
                _currentCameraIndex = 0;
                _cameras[_currentCameraIndex].SetActive(true);
                _mainCamera.SetActive(false);
                _isMainCameraActive = false;
                return;
            }
        }

        public void SwitchToMain()
        {
            _isMainCameraActive = true;
            _mainCamera.SetActive(true);
            if (_cameras.Count != 0)
            {
                _cameras[_currentCameraIndex].SetActive(false);
            }
        }

        private void Start()
        {
            _mainCamera = GameObject.Find("Main Camera");
            _isMainCameraActive = true;
            _currentCameraIndex = 0;
            cameraUI.text = "Main";
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Switch();
                if (_isMainCameraActive)
                {
                    cameraUI.text = "Main";
                }
                else
                {
                    cameraUI.text = _cameras[_currentCameraIndex].transform.parent.name;
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                SwitchToMain();
                cameraUI.text = "Main";
            }
        }
    }
}
