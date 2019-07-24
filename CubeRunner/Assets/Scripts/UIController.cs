using Assets.Scripts.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIController : MonoBehaviour
    {
        public PlayerController Player;
        public GameController GameController;
        public TMP_Text CurrentInGameScoreText;

        public TMP_Text EndScreenCurrentScoreText;
        public TMP_Text EndScreenBestScoreText;

        public TMP_Text SplashScreenBestScoreText;


        public GameObject splashScreen;
        public GameObject inGameUiScreen;
        public GameObject endScreen;
        public GameObject debugScreen;

        private bool firstGamePlay = true;

        public void SlideSplashScreenIn()
        {
            //Set best score for splash screen
            SplashScreenBestScoreText.text = GameController.GetCurrentBestScore().ToString();

            //Slide main menu into place
            splashScreen.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.25f);
        }

        public void ShowEndScreen()
        {
            EndScreenCurrentScoreText.text = GameController.GetCurrentScore().ToString();
            EndScreenBestScoreText.text = GameController.GetCurrentBestScore().ToString();

            //Slide GameUi out
            inGameUiScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 3000f), 0.25f, true);
            //Slide end screen in
            endScreen.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.25f, true);

            firstGamePlay = false;
        }

        public void StartPlayingAndShowGamePlayUi()
        {
            //Slide Splash screen out
            splashScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-3000f, 0f), 0.25f);
            //Slide end screen out
            endScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, -3000f), 0.25f);

            //Slide GameUi in
            inGameUiScreen.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.25f);
        }

        public void GotoSettingsMenu()
        {
            if (firstGamePlay)
            {
                //Slide Splash screen out
                splashScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-3000f, 0f), 0.25f);
                //Slide debug in
                debugScreen.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.25f);
            }
            else
            {
                //Slide end screen out
                endScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-3000f, 0f), 0.25f);
                //Slide debug in
                debugScreen.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.25f);
            }
        }

        public void ExitSettingsMenu()
        {
            if (firstGamePlay)
            {
                //Slide Splash screen in
                splashScreen.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.25f);
                //Slide debug out
                debugScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(3000f, 0f), 0.25f);
            }
            else
            {
                //Slide end screen in
                endScreen.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.25f);
                //Slide debug out
                debugScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(3000f, 0f), 0.25f);
            }
        }

        private void Update()
        {
            CurrentInGameScoreText.text = GameController.GetCurrentScore().ToString();

        }

    }
}