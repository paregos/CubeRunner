using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{

    private float _blockWidth = (float)Math.Pow(2f, 0.5f);
    private float _halfblockWidth = ((float)Math.Pow(2f, 0.5f))/2f;

    private Queue<float> _distanceToSpawnNextWallQueue = new Queue<float>();

    public int initialRowCount = 5;
    public int initialRowWidth = 7;

    public GameObject wallBlockPrefab;
    public GameObject floorBlockPrefab;

    void Update()
    {
		//Check queue each frame against ball position
    }

    // Use this for initialization
    void Start ()
    {
	    SpawnStarterRows();
    }

    private void SpawnStarterRows()
    {
	    for (int i = 0; i < initialRowCount; i++)
	    {
		    SpawnNextRow(i);
	    }
	}

    public void SpawnNextRow(float rowNumber)
    {
        var isSmallRow = rowNumber % 2 == 1;
        var floorRowWidth = isSmallRow ? initialRowWidth - 1 : initialRowWidth;

        SpawnWalls(rowNumber, isSmallRow);

        //Floor blocks
        for (int i = 0; i < floorRowWidth; i++)
        {
            FloorBlockController floorBlock;
            if (isSmallRow)
            {
                floorBlock = Instantiate(floorBlockPrefab, 
                        new Vector3(
                            rowNumber * _blockWidth / 2f,
                            0.1f,
                            _blockWidth + _halfblockWidth + i * _blockWidth),
                        Quaternion.Euler(0, 45, 0))
                    .GetComponent<FloorBlockController>();
            }
            else
            {
                floorBlock = Instantiate(floorBlockPrefab, 
                        new Vector3(
                            rowNumber * _halfblockWidth,
                            0.1f,
                            _blockWidth + i * _blockWidth),
                        Quaternion.Euler(0, 45, 0))
                    .GetComponent<FloorBlockController>();
            }
            SetBlockColor(floorBlock.gameObject);
        }
    }

    private void SpawnWalls(float rowNumber, bool isSmallRow)
    {
        Vector3 leftBlockFinalPosition;
        Vector3 rightBlockFinalPosition;

        if (isSmallRow)
        {
            leftBlockFinalPosition = new Vector3(
                rowNumber * _blockWidth / 2f,
                0.5f,
                _halfblockWidth);
            rightBlockFinalPosition = new Vector3(
                rowNumber * _blockWidth / 2f,
                0.5f,
                initialRowWidth * _blockWidth + _halfblockWidth);
        }
        else
        {
            leftBlockFinalPosition = new Vector3(
                rowNumber * _halfblockWidth,
                0.5f,
                0);
            rightBlockFinalPosition = new Vector3(
                rowNumber * _halfblockWidth,
                0.5f,
                _blockWidth + initialRowWidth * _blockWidth);
        }

        WallBlockController leftWallBlock = Instantiate(wallBlockPrefab, new Vector3(leftBlockFinalPosition.x, leftBlockFinalPosition.y - 10, leftBlockFinalPosition.z), Quaternion.Euler(0, 45, 0))
            .GetComponent<WallBlockController>();
        WallBlockController rightWallBlock = Instantiate(wallBlockPrefab, new Vector3(rightBlockFinalPosition.x, rightBlockFinalPosition.y - 10, rightBlockFinalPosition.z), Quaternion.Euler(0, 45, 0))
            .GetComponent<WallBlockController>();

        SetBlockColor(leftWallBlock.gameObject);
        SetBlockColor(rightWallBlock.gameObject);

        leftWallBlock.MoveToPosition(leftBlockFinalPosition);
        rightWallBlock.MoveToPosition(rightBlockFinalPosition);
    }

    private void SetBlockColor(GameObject block)
    {
        block.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

}
