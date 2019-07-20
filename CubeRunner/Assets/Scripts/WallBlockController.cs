using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;

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

    private void SetWallColor()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.gray;
    }
}
