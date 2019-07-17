using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseCubeController : MonoBehaviour
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
        transform.DOMove(position, 5);
        transform.DOShakeRotation(10);
        baseMaterial.DOFade(255, 10);
    }
}
