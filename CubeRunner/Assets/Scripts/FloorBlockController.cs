using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class FloorBlockController : BlockController
    {
        // Use this for initialization
        void Start()
        {
            SetBlockRandomColor(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            DestroyIfFarFromPlayer();
        }
    }
}