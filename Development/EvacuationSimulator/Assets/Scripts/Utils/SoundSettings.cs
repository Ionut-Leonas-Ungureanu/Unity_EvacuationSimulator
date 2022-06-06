using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class SoundSettings
    {
        private float _introVolume = 0.1f;
        public float IntroVolume 
        {
            get => _introVolume;
            set
            {
                _introVolume = value;
                if(_introAudioSource != null)
                {
                    _introAudioSource.volume = _introVolume;
                }
            }
        }

        private float _alarmVolume = 0.0f;
        public float AlarmVolume
        {
            get => _alarmVolume;
            set
            {
                _alarmVolume = value;
                if (_alarmAudioSource != null)
                {
                    _alarmAudioSource.volume = _alarmVolume;
                }
            }
        }

        private AudioSource _introAudioSource;
        public AudioSource IntroAudioSource 
        { 
            get => _introAudioSource;
            set
            {
                _introAudioSource = value;
                _introAudioSource.volume = _introVolume;
            }
        }

        private AudioSource _alarmAudioSource;
        public AudioSource AlarmAudioSource
        {
            get => _alarmAudioSource;
            set
            {
                _alarmAudioSource = value;
                _alarmAudioSource.volume = _alarmVolume;
            }
        }
    }
}
