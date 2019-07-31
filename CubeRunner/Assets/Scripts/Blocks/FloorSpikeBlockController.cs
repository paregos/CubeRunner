using System.Collections;
using Assets.Scripts.Player;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Blocks
{
    public class FloorSpikeBlockController : BlockController
    {
        private bool _spikesEnabled;

        public GameObject Spikes;

        public float randomOffset;

        private bool _triggeredEnd;

        public void Start()
        {
            _spikesEnabled = Random.value < 0.5f;
            randomOffset = Random.value < 0.5f ? 0.5f : 0;
            StartCoroutine(spikeCorountine());
            _triggeredEnd = false;
        }

        public override void HandlePlayerInteraction(GameObject player)
        {
            if (_spikesEnabled)
            {
                if (!_triggeredEnd)
                {
                    _triggeredEnd = true;
                    player.transform.position = new Vector3(gameObject.transform.position.x,player.transform.position.y, gameObject.transform.position.z);
                    player.GetComponent<PlayerController>().GetSpiked();
                    GameObject.Find("GameController").GetComponent<GameController>().StopGame();
                }
            }
        }

        public void Update()
        {
            DestroyIfFarFromPlayer();
        }

        IEnumerator spikeCorountine()
        {
            while (true)
            {
                ToggleSpikes();
                if (_spikesEnabled)
                {
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
               
            }
        }

        public void ToggleSpikes()
        {
            if (_spikesEnabled)
            {
                DisableSpikes();
            }
            else
            {
                EnableSpikes();
            }
        }

        public void DisableSpikes()
        {
            _spikesEnabled = false;
            Spikes.transform.DOLocalMoveY(-.66f, 0.1f);
        }

        public void EnableSpikes()
        {
            Spikes.transform.DOLocalMoveY(0f, 0.1f);
            _spikesEnabled = true;
        }
    }
}