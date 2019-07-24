using Assets.Scripts.Persistence;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {

        public LevelGenerator LevelGenerator;
        public UIController UiController;
        public PlayerController player;
        public CameraFollow camera;

        private bool _firstPlay = true;

        public int bestScoreThisInstance = 0;

        public void Start()
        {
            bestScoreThisInstance = SaveManager.Instance.state.HighScore;
            
            UiController.SlideSplashScreenIn();
        }

        public void ResetGame()
        {
            if (!_firstPlay)
            {
                //Removing all walls and blocks
                foreach (var wall in GameObject.FindGameObjectsWithTag("Wall"))
                {
                    Destroy(wall);
                }

                foreach (var block in GameObject.FindGameObjectsWithTag("Block"))
                {
                    Destroy(block);
                }
                LevelGenerator.ResetLevel();
            }
            else
            {
                _firstPlay = false;
            }

            camera.Reset();
            player.ResetPlayer();
        }

        public void StopGame()
        {
            // Update highscore
            bestScoreThisInstance = bestScoreThisInstance > player.GetNumberOfRowsPassed()
                ? bestScoreThisInstance
                : player.GetNumberOfRowsPassed();

            player.StopPlayer();
            UiController.ShowEndScreen();
        }

        private void SaveGame()
        {
            if (bestScoreThisInstance > SaveManager.Instance.state.HighScore)
            {
                SaveManager.Instance.state.SetState(bestScoreThisInstance);
                SaveManager.Instance.Save();
            }
        }

        void OnApplicationPause(bool gameIsPaused)
        {
            if (gameIsPaused)
            {
                Debug.Log("game paused");
                SaveGame();
            }
        }

        void OnApplicationQuit()
        {
            SaveGame();
        }

        public int GetCurrentScore()
        {
            return player.GetNumberOfRowsPassed();
        }

        public int GetCurrentBestScore()
        {
            return bestScoreThisInstance;
        }

    }
}