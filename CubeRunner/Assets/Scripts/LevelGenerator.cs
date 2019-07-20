using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    private Queue<float> _distanceToSpawnNextWallQueue = new Queue<float>();
    private List<BlockController> _previousRow = new List<BlockController>();
    private int _rowsSpawned = 0;

    public int initialRowWidth = 5;
    public float hazardPercentage = 0.1f;

    public GameObject wallBlockPrefab;
    public GameObject floorBlockPrefab;
    public GameObject holeBlockPrefab;

    public GameObject mainPlayer;

    void Update()
    {
		//Check queue each frame against ball position
        var playerXPos = mainPlayer.transform.position.x;

        while (_distanceToSpawnNextWallQueue.Peek() < playerXPos)
        {
            _distanceToSpawnNextWallQueue.Dequeue();
            SpawnNextRow(_rowsSpawned);
        }
    }

    public void ResetLevel()
    {
        _rowsSpawned = 0;
        SpawnStarterRows();
    }

    // Use this for initialization
    void Start ()
    {
	    SpawnStarterRows();
    }

    private void SpawnStarterRows()
    {
	    for (int i = 0; i < GameConstants.rowLeadLength; i++)
	    {
		    SpawnNextRow(i);
	    }
	}

    public void SpawnNextRow(float rowNumber)
    {
        var isSmallRow = rowNumber % 2 == 1;
        var floorRowWidth = isSmallRow ? initialRowWidth - 1 : initialRowWidth;

        SpawnWalls(rowNumber, isSmallRow);

        //GenerateNextRowHazards
        var nextRowHazardFlags = GenerateNextRowHazardFlags();
        _previousRow = new List<BlockController>();

        //Floor blocks
        for (int i = 0; i < nextRowHazardFlags.Count; i++)
        {
            //Spawn floor block at random position down the track
            var blockInitialPosition = GenerateRandomVector3BeyondXValue(rowNumber * GameConstants.halfblockWidth);
            BlockController floorBlock;
            //Create random floor block 
            if (nextRowHazardFlags[i])
            {
                floorBlock = Instantiate(holeBlockPrefab, blockInitialPosition, Quaternion.Euler(0, 45, 0))
                    .GetComponent<HoleBlockController>();
            }
            else
            {
                floorBlock = Instantiate(floorBlockPrefab, blockInitialPosition, Quaternion.Euler(0, 45, 0))
                    .GetComponent<FloorBlockController>();
            }

            //Move Floor block into correct position
            if (isSmallRow)
            {
                floorBlock.MoveToPosition(new Vector3(
                    rowNumber * GameConstants.halfblockWidth,
                    0.1f,
                    GameConstants.blockWidth + GameConstants.halfblockWidth + i * GameConstants.blockWidth),
                    rowNumber <= GameConstants.rowLeadLength);
            }
            else
            {
                floorBlock.MoveToPosition(new Vector3(
                    rowNumber * GameConstants.halfblockWidth,
                    0.1f,
                    GameConstants.blockWidth + i * GameConstants.blockWidth),
                    rowNumber <= GameConstants.rowLeadLength);
            }

            //Add block to previous row list
            _previousRow.Add(floorBlock);
        }

        _rowsSpawned++;
        _distanceToSpawnNextWallQueue.Enqueue(rowNumber*GameConstants.halfblockWidth + GameConstants.halfblockWidth);
    }

    private void SpawnWalls(float rowNumber, bool isSmallRow)
    {
        Vector3 leftBlockFinalPosition;
        Vector3 rightBlockFinalPosition;

        if (isSmallRow)
        {
            leftBlockFinalPosition = new Vector3(
                rowNumber * GameConstants.blockWidth / 2f,
                0.5f,
                GameConstants.halfblockWidth);
            rightBlockFinalPosition = new Vector3(
                rowNumber * GameConstants.blockWidth / 2f,
                0.5f,
                initialRowWidth * GameConstants.blockWidth + GameConstants.halfblockWidth);
        }
        else
        {
            leftBlockFinalPosition = new Vector3(
                rowNumber * GameConstants.halfblockWidth,
                0.5f,
                0);
            rightBlockFinalPosition = new Vector3(
                rowNumber * GameConstants.halfblockWidth,
                0.5f,
                GameConstants.blockWidth + initialRowWidth * GameConstants.blockWidth);
        }

        var gameobj1 = Instantiate(wallBlockPrefab,
            new Vector3(leftBlockFinalPosition.x, leftBlockFinalPosition.y - 10, leftBlockFinalPosition.z),
            Quaternion.Euler(0, 45, 0));

        WallBlockController leftWallBlock = gameobj1
            .GetComponent<WallBlockController>();

        WallBlockController rightWallBlock = Instantiate(wallBlockPrefab, new Vector3(rightBlockFinalPosition.x, rightBlockFinalPosition.y - 10, rightBlockFinalPosition.z), Quaternion.Euler(0, 45, 0))
            .GetComponent<WallBlockController>();

        leftWallBlock.MoveToPosition(leftBlockFinalPosition, rowNumber <= GameConstants.rowLeadLength);
        rightWallBlock.MoveToPosition(rightBlockFinalPosition, rowNumber <= GameConstants.rowLeadLength);
    }


    private Vector3 GenerateRandomVector3BeyondXValue(float x)
    {
        return new Vector3(x + Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10));
    }

    private List<bool> GenerateNextRowHazardFlags()
    {
        List<bool> nextRowHazardFlags = new List<bool>();
        var nextRowLength = _previousRow.Count == 0 ? initialRowWidth : _previousRow.Count % 2 == 0 ? initialRowWidth - 1 : initialRowWidth;

        //For starter rows make them hazard free
        if (_rowsSpawned < GameConstants.rowLeadLength)
        {
            for (int i = 0; i < nextRowLength; i++)
            {
                nextRowHazardFlags.Add(false);
            }

            return nextRowHazardFlags;
        }

        // Add random hazards
        for (int i = 0; i < nextRowLength; i++)
        {
            nextRowHazardFlags.Add(Random.value < hazardPercentage);
        }

        // Clear hazards so there is a viable path
        var safeNextRowIndexs = GenerateNextRowSafeIndexs();
        var foundSafePath = false;

        // Check if a safe path exists
        foreach (var i in safeNextRowIndexs)
        {
            if (nextRowHazardFlags[i] == false)
            {
                foundSafePath = true;
                break;
            }
        }

        // If no safe path found manually make one from safe next row indexs
        //TODO - improve selection
        if (!foundSafePath)
        {
            nextRowHazardFlags[safeNextRowIndexs.First()] = false;
        }

        return nextRowHazardFlags;
    }

    private HashSet<int> GenerateNextRowSafeIndexs()
    {
        var safeNextRowIndexs = new HashSet<int>();
        for (int i = 0; i < _previousRow.Count; i++)
        {
            if (!_previousRow[i].isHazard)
            {
                if (i > 0 || !isLargeRow(_previousRow.Count))
                {
                    safeNextRowIndexs.Add(i - i);
                }

                if (i < _previousRow.Count-1 || !isLargeRow(_previousRow.Count))
                {
                    safeNextRowIndexs.Add(i + i);
                }
            }
        }

        return safeNextRowIndexs;
    }

    public bool isLargeRow(int rowLength)
    {
        return rowLength % 2 == 0;
    }

}
