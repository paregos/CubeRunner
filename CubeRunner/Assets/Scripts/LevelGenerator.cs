using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public GameObject baseCube;

    // Use this for initialization
    void Start () {
        for (int i = 1; i < 100; i++)
        {
            Vector3 leftCubeFinalPosition = new Vector3(i - 3f, 0.5f, i + 3f);
            Vector3 rightCubeFinalPosition = new Vector3(i + 3f, 0.5f, i - 3f);

            BaseCubeController leftCube = Instantiate(
                    baseCube, 
                    new Vector3(leftCubeFinalPosition.x, leftCubeFinalPosition.y-10, leftCubeFinalPosition.z),
                    Quaternion.identity)
                .GetComponent<BaseCubeController>();
            BaseCubeController rightCube = Instantiate(
                baseCube,
                new Vector3(rightCubeFinalPosition.x, rightCubeFinalPosition.y - 10, rightCubeFinalPosition.z),
                Quaternion.identity)
                .GetComponent<BaseCubeController>();

            leftCube.MoveToPosition(leftCubeFinalPosition);
            rightCube.MoveToPosition(rightCubeFinalPosition);
        }
	}

}
