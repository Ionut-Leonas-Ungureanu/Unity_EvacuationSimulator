using Assets.Scripts.DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MainMenu
{
    class ResultControllers : MonoBehaviour
    {
        public void StopSimulation()
        {
            SimulationConfigurator.Instance.StopSimulation = true;
            //gameObject.SetActive(false);
        }

        #region Back Button

        public void BackToMainMenu()
        {
            if (!SimulationConfigurator.Instance.SimulationIsRunning)
            {
                SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            }
            //gameObject.SetActive(false);
        }

        #endregion

        #region Quit Button

        public void QuitButton()
        {
            if (!SimulationConfigurator.Instance.SimulationIsRunning)
            {
                Application.Quit();
            }
            //gameObject.SetActive(false);
        }

        #endregion
    }
}
