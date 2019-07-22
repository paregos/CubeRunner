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

        public bool isValidPath;

        private int _rowDistanceBeforeDestruction = 9;

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
            var playerController = player.GetComponent<PlayerController>();

            if (Math.Abs(gameObject.transform.position.x - playerPosition.x) < GameConstants.switchAreaValue && Math.Abs(gameObject.transform.position.z - playerPosition.z) < GameConstants.switchAreaValue)
            {
                //HighlightPlayer
                playerController.TurnOnHighLight();
                HandlePlayerInteraction(player);
            }
            else
            {
                //UnHighlightPlayer
                if (PlayerCenterIsWithinBlock(playerPosition))
                {
                    playerController.TurnOffHighlight();
                }
            }
        }

        public bool PlayerCenterIsWithinBlock(Vector3 playerPosition)
        {
            return (transform.position.x + GameConstants.halfblockWidth > playerPosition.x &&
                    transform.position.x - GameConstants.halfblockWidth < playerPosition.x &&
                    transform.position.z + GameConstants.halfblockWidth > playerPosition.z &&
                    transform.position.z - GameConstants.halfblockWidth < playerPosition.z);
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
            if (isValidPath && GameConstants.highlightValidPath)
            {
                return;
            }

            if (GameConstants.setRandomColorBlocks)
            {
                block.GetComponent<Renderer>().material.SetColor("_EmissionColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            }
        }

        public void SetValidPathBlockColor(GameObject block)
        {
            block.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
        }
    }
}