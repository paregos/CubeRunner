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
                player.GetComponent<Rigidbody>().velocity = Vector3.down;
                GameObject.Find("GameController").GetComponent<GameController>().StopGame();
                _triggeredEnd = true;
            }
        }
    }
}