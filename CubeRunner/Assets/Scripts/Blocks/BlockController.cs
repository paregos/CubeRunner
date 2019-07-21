using System;
using Assets.Scripts.Player;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Blocks
{
    public class BlockController : MonoBehaviour
    {
        public Material baseMaterial;

        private int _rowDistanceBeforeDestruction = 5;

        public virtual bool IsHazard()
        {
            return false;
        }

        void Start()
        {
        }

        void FixedUpdate()
        {
            var player = GameObject.Find("Player");
            var playerPosition = player.transform.position;
            if (Math.Abs(gameObject.transform.position.x - playerPosition.x) < 0.1 && Math.Abs(gameObject.transform.position.z - playerPosition.z) < 0.1)
            {
                HandlePlayerInteraction(player);
            }
        }

        public virtual void HandlePlayerInteraction(GameObject player)
        {
            var playerController = player.GetComponent<PlayerController>();
            playerController.ChangeDirectionsIfNeeded(transform.position.x, transform.position.z);
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