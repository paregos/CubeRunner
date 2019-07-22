using Assets.Scripts.Player;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIController : MonoBehaviour
    {
        public PlayerController Player;
        public TMP_Text ScoreText;
        public GameObject endScreen;

        public void ShowEndScreen()
        {
            endScreen.SetActive(true);
        }

        public void HideEndScreen()
        {
            endScreen.SetActive(false);
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