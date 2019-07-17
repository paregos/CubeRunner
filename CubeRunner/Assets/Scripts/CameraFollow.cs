using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject player;


    void Update()
    {
        // Position
        Vector3 pos = player.transform.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }
}
