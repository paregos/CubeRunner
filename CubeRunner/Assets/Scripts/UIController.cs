using Assets.Scripts.Player;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIController : MonoBehaviour
    {
        public PlayerController Player;
        public TMP_Text ScoreText;

        public GameObject splashScreen;
        public GameObject inGameUiScreen;
        public GameObject endScreen;
        public GameObject debugScreen;

        private bool firstGamePlay = true;

        public void ShowEndScreen()
        {
            inGameUiScreen.SetActive(false);
            endScreen.SetActive(true);
            firstGamePlay = false;
        }

        public void StartPlayingAndShowGamePlayUi()
        {
            endScreen.SetActive(false);
            debugScreen.SetActive(false);
            splashScreen.SetActive(false);
            inGameUiScreen.SetActive(true);
        }

        public void ToggleSettingsMenu()
        {
            debugScreen.SetActive(!debugScreen.activeSelf);

            if (firstGamePlay)
            {
                splashScreen.SetActive(!debugScreen.activeSelf);
            }
            else
            {
                endScreen.SetActive(!debugScreen.activeSelf);
            }
        }

        private void Update()
        {
            ScoreText.text = CalculateScore().ToString();
        }

        private int CalculateScore()
        {
            return Player.GetNumberOfRowsPassed();
        }

    }
}