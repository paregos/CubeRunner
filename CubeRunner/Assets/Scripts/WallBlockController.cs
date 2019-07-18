using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;

public class WallBlockController : MonoBehaviour, IBlockController
{
    public Material baseMaterial;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveToPosition(Vector3 position)
    {
        transform.DOMove(position, 2);
        transform.DOShakeRotation(2);
        baseMaterial.DOFade(255, 3);
    }
}
