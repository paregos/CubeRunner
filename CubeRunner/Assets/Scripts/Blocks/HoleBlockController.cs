using UnityEngine;

namespace Assets.Scripts.Blocks
{
    public class HoleBlockController : BlockController
    {
        public override bool IsHazard()
        {
            return true;
        }

        public override void HandlePlayerInteraction(GameObject player)
        {
            player.GetComponent<Rigidbody>().velocity = Vector3.down;
            GameObject.Find("GameController").GetComponent<GameController>().StopGame();
        }
    }
}