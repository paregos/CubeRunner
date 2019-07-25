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

            if (Math.Abs(gameObject.transform.position.x - playerPosition.x) < GameConstants.switchAreaValue && Math.Abs(gameObject.transform.position.z - playerPosition.z) < GameConstants.switchAreaValue)
            {
                HandlePlayerInteraction(player);
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

//        public void SetBlockRandomColor(GameObject block)
//        {
//            if (isValidPath && GameConstants.highlightValidPath)
//            {
//                return;
//            }
//
//            if (GameConstants.setRandomColorBlocks)
//            {
//                var color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
//                block.GetComponent<Renderer>().material.color = color;
//                block.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
//            }
//        }

        public void SetColor(Color baseColor, bool isOffColor, bool shouldHighLightYellow)
        {
            var material = gameObject.GetComponent<Renderer>().material;
            if (shouldHighLightYellow)
            {
                material.color = Color.yellow;
                material.SetColor("_EmissionColor", Color.yellow);
                return;
            }
           
            if (isOffColor)
            {
                material.color = baseColor;
                material.SetColor("_EmissionColor", baseColor);
            }
            else
            {
                var offColor = new Color(baseColor.r / 1.3f, baseColor.g / 1.3f, baseColor.b/ 1.3f);
                material.color = offColor;
                material.SetColor("_EmissionColor", offColor);
            }
        }

    }
}