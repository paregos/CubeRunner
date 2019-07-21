using Assets.Scripts.Player;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIController : MonoBehaviour
    {
        public PlayerController Player;
        public TMP_Text ScoreText;
        public GameObject playButton;

        public void ShowEndScreen()
        {
            playButton.SetActive(true);
        }

        public void HideEndScreen()
        {
            playButton.SetActive(false);
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