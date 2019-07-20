using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockController : MonoBehaviour
    {
        public Material baseMaterial;

        public bool isHazard = false;
        
        private int _rowDistanceBeforeDestruction = 15;

        void Start()
        {
        }

        public void DestroyIfFarFromPlayer()
        {
            if (_rowDistanceBeforeDestruction * GameConstants.halfblockWidth <= GameObject.Find("Player").transform.position.x - transform.position.x)
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }

        public void MoveToPosition(Vector3 position, bool moveInstantly = false)
        {
            var playerVelocityX = GameObject.Find("Player").GetComponent<Rigidbody>().velocity.x;

            // Hack to make initial platforms spawn right away
            var animationTime = moveInstantly ? 0.1f : ((float)GameConstants.rowLeadLength / 4f) / playerVelocityX;

            Debug.Log(animationTime +"animationTimer");

            transform.DOMove(position, animationTime);
            transform.DOShakeRotation(animationTime);
            baseMaterial.DOFade(255, animationTime);
        }

        public void SetBlockRandomColor(GameObject block)
        {
            block.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
    }
}