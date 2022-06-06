using Assets.Scripts.DesignPatterns.Singleton;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    class IntroAudioSource : MonoBehaviour
    {
        public AudioSource AudioSource;

        private void Start()
        {
            SimulationConfigurator.Instance.SoundSettings.IntroAudioSource = AudioSource;
        }
    }
}
