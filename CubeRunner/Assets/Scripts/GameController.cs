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

        private bool firstPlay = true;

        public void ResetGame()
        {
            if (!firstPlay)
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
                firstPlay = false;
            }

            camera.Reset();
            player.ResetPlayer();
        }

        public void StopGame()
        {
            player.StopPlayer();
            UiController.ShowEndScreen();
        }

    }
}