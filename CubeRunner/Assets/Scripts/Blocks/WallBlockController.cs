using UnityEngine;

namespace Assets.Scripts.Blocks
{
    public class WallBlockController : BlockController
    {
        // Use this for initialization
        void Start ()
        {
            SetWallColor();
        }
	
        // Update is called once per frame
        void Update ()
        {
            DestroyIfFarFromPlayer();
        }

        public override void HandlePlayerInteraction(GameObject player)
        {
            return;
        }

        private void SetWallColor()
        {
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
        }
    }
}
