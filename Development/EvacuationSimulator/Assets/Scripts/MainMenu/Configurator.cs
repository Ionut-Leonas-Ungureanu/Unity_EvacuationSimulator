using Assets.Scripts.DesignPatterns.Singleton;
using Assets.Scripts.Utils;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class Configurator : MonoBehaviour
    {

        #region Sliders
        private Slider NoBots_slider;

        private Slider NoRounds_slider;
        private Slider WaitToDisplayResults_slider;

        private Slider FireNodeScale_slider;
        private Slider FireSpreadingSpeed_slider;

        private Slider IntroSoundVolume_slider;
        private Slider AlarmSoundVolume_slider;

        private Toggle Train_checkbox;
        private Slider ClearPathSpeed_slider;
        #endregion

        private void Start()
        {
            #region Slider Settings

            NoBots_slider = GameObject.Find("NumberOfBots_slider").GetComponent<Slider>();
            NoBots_slider.value = SimulationConfigurator.Instance.BotsSettings.NumberBots;
            NoBots_slider.onValueChanged.AddListener(SetNumberOfBots);

            NoRounds_slider = GameObject.Find("NumberOfRuns_slider").GetComponent<Slider>();
            NoRounds_slider.value = SimulationConfigurator.Instance.SimulationSettings.NumberOfRuns;
            NoRounds_slider.onValueChanged.AddListener(SetNumberOfRuns);

            WaitToDisplayResults_slider = GameObject.Find("WaitToDisplayResults_slider").GetComponent<Slider>();
            WaitToDisplayResults_slider.value = SimulationConfigurator.Instance.SimulationSettings.WaitToDisplayResults;
            WaitToDisplayResults_slider.onValueChanged.AddListener(SetWaitToDisplayResults);

            FireNodeScale_slider = GameObject.Find("FireScale_slider").GetComponent<Slider>();
            FireNodeScale_slider.value = SimulationConfigurator.Instance.FireSettings.FireNodeScale;
            FireNodeScale_slider.onValueChanged.AddListener(SetFireNodeScale);

            FireSpreadingSpeed_slider = GameObject.Find("FireSpreadingSpeed_slider").GetComponent<Slider>();
            FireSpreadingSpeed_slider.value = SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed;
            FireSpreadingSpeed_slider.onValueChanged.AddListener(SetFireSpreadingSpeed);

            IntroSoundVolume_slider = GameObject.Find("IntroSoundVolume_slider").GetComponent<Slider>();
            //IntroSoundVolume_slider.value = SimulationConfigurator.Instance.SoundSettings.IntroVolume;
            IntroSoundVolume_slider.onValueChanged.AddListener((value) => { SetIntroSoundVolume(value); });

            AlarmSoundVolume_slider = GameObject.Find("AlarmSoundVolume_slider").GetComponent<Slider>();
            AlarmSoundVolume_slider.value = SimulationConfigurator.Instance.SoundSettings.AlarmVolume;
            AlarmSoundVolume_slider.onValueChanged.AddListener(SetAlarmSoundVolume);

            ClearPathSpeed_slider = GameObject.Find("ClearPathSpeed_slider").GetComponent<Slider>();
            SimulationConfigurator.Instance.MiscellaneousSettings.ClearPathSpeed = (int)ClearPathSpeed_slider.value;
            ClearPathSpeed_slider.onValueChanged.AddListener(SetClearPathSpeed);
            #endregion

            Train_checkbox = GameObject.Find("Train_checkbox").GetComponent<Toggle>();
            Train_checkbox.onValueChanged.AddListener(SetTrain);
        }

        #region Bots

        public void SetNumberOfBots(float value)
        {
            SimulationConfigurator.Instance.BotsSettings.NumberBots = (uint)value;
        }

        #endregion

        #region Fire

        public void SetFireNodeScale(float value)
        {
            SimulationConfigurator.Instance.FireSettings.FireNodeScale = (uint)value;
        }

        public void SetFireSpreadingSpeed(float value)
        {
            SimulationConfigurator.Instance.FireSettings.FireSpreadingSpeed = value;
        }

        #endregion

        #region Sound

        public void SetIntroSoundVolume(float value)
        {
            SimulationConfigurator.Instance.SoundSettings.IntroVolume = value;
        }
        public void SetAlarmSoundVolume(float value)
        {
            SimulationConfigurator.Instance.SoundSettings.AlarmVolume = value;
        }

        #endregion

        #region Multiple Runs

        public void SetNumberOfRuns(float value)
        {
            SimulationConfigurator.Instance.SimulationSettings.NumberOfRuns = (int)value;
        }

        public void SetWaitToDisplayResults(float value)
        {
            SimulationConfigurator.Instance.SimulationSettings.WaitToDisplayResults = value;
        }

        public void SetClearPathSpeed(float value)
        {
            SimulationConfigurator.Instance.MiscellaneousSettings.ClearPathSpeed = (int)value;
        }

        #endregion

        public void SetTrain(bool value)
        {
            SimulationConfigurator.Instance.MiscellaneousSettings.Train = value;
        }
    }
}
