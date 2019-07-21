using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {

        public LevelGenerator LevelGenerator;
        public PlayerController player;

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

           
            player.ResetPlayer();
        }

    }
}