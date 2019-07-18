using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{


    private float _blockWidth = (float)Math.Pow(2f, 0.5f);
    private float _halfblockWidth = ((float)Math.Pow(2f, 0.5f))/2f;

    public int initialRowCount = 10;
    public int initialRowWidth = 7;

    public GameObject wallBlock;

    // Use this for initialization
    void Start () {
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
        //TODO - make this flexible
        for (int i = 0; i < floorRowWidth; i++)
        {
            if (isSmallRow)
            {
                FloorBlockController floorBlock = Instantiate(wallBlock, 
                        new Vector3(
                            rowNumber * _blockWidth / 2f,
                            0.1f,
                            _blockWidth + _halfblockWidth + i * _blockWidth),
                        Quaternion.Euler(0, 45, 0))
                    .GetComponent<FloorBlockController>();
            }
            else
            {
                FloorBlockController floorBlock = Instantiate(wallBlock, 
                        new Vector3(
                            rowNumber * _halfblockWidth,
                            0.1f,
                            _blockWidth + i * _blockWidth),
                        Quaternion.Euler(0, 45, 0))
                    .GetComponent<FloorBlockController>();
            }
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

        WallBlockController leftWallBlock = Instantiate(wallBlock, new Vector3(leftBlockFinalPosition.x, leftBlockFinalPosition.y - 10, leftBlockFinalPosition.z), Quaternion.Euler(0, 45, 0))
            .GetComponent<WallBlockController>();
        WallBlockController rightWallBlock = Instantiate(wallBlock, new Vector3(rightBlockFinalPosition.x, rightBlockFinalPosition.y - 10, rightBlockFinalPosition.z), Quaternion.Euler(0, 45, 0))
            .GetComponent<WallBlockController>();

        leftWallBlock.MoveToPosition(leftBlockFinalPosition);
        rightWallBlock.MoveToPosition(rightBlockFinalPosition);
    }

}
