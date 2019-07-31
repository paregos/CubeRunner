using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Blocks
{
    public class HoleBlockController : BlockController
    {
        private bool _triggeredEnd = false;

        public override bool IsHazard()
        {
            return true;
        }

        public override void HandlePlayerInteraction(GameObject player)
        {
            if (!_triggeredEnd)
            {
                player.transform.position = new Vector3(gameObject.transform.position.x, player.transform.position.y, gameObject.transform.position.z);
                player.GetComponent<PlayerController>().FallDownHole();
                GameObject.Find("GameController").GetComponent<GameController>().StopGame();
                _triggeredEnd = true;
            }
        }
    }
}