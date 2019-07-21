using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Blocks;
using Assets.Scripts.Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    private Queue<float> _distanceToSpawnNextWallQueue = new Queue<float>();
    private List<PreviousBlockState> _previousRowState = new List<PreviousBlockState>();
    private int _rowsSpawned = 0;
    private System.Random rnd = new System.Random();

    private int initialRowOddWidth = 5;
    public float hazardPercentage = 0.1f;

    public GameObject wallBlockPrefab;
    public GameObject floorBlockPrefab;
    public GameObject holeBlockPrefab;

    public GameObject mainPlayer;

    // Use this for initialization
    void Start()
    {
        ResetLevel();
    }

    void Update()
    {
		//Check queue each frame against ball position
        var playerXPos = mainPlayer.transform.position.x;

        while (_distanceToSpawnNextWallQueue.Peek() < playerXPos)
        {
            _distanceToSpawnNextWallQueue.Dequeue();
            SpawnNextRow(_rowsSpawned);
            mainPlayer.GetComponent<PlayerController>().IncrementNumberOfRowsPassed();
        }
    }

    public void ResetLevel()
    {
        _rowsSpawned = 0;
        _previousRowState = new List<PreviousBlockState>();
        _distanceToSpawnNextWallQueue =  new Queue<float>();
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
        var floorRowWidth = isSmallRow ? initialRowOddWidth - 1 : initialRowOddWidth;

        SpawnWalls(rowNumber, isSmallRow);

        //GenerateNextRowHazards
        var nextRowHazardFlags = GenerateNextRowHazardFlags();

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
                initialRowOddWidth * GameConstants.blockWidth + GameConstants.halfblockWidth);
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
                GameConstants.blockWidth + initialRowOddWidth * GameConstants.blockWidth);
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

    private List<bool> GenerateNextRowHazardFlags()
    {
        List<bool> nextRowHazardFlags = new List<bool>();
        List<int> safeNextRowIndexs = new List<int>();
        var nextRowLength = _previousRowState.Count == 0 ? initialRowOddWidth : isLargeRow(_previousRowState.Count) ? initialRowOddWidth - 1 : initialRowOddWidth;

        //For starter rows make them hazard free
        if (_rowsSpawned < GameConstants.rowLeadLength)
        {
            for (int i = 0; i < nextRowLength; i++)
            {
                nextRowHazardFlags.Add(false);
                safeNextRowIndexs.Add(i);
            }

            SetPreviousRowState(nextRowHazardFlags, safeNextRowIndexs);

            return nextRowHazardFlags;
        }

        // Add random hazards
        for (int i = 0; i < nextRowLength; i++)
        {
            nextRowHazardFlags.Add(Random.value < hazardPercentage);
        }

        // Clear hazards so there is a viable path
        safeNextRowIndexs = GenerateNextRowSafeIndexs().ToList();
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
            nextRowHazardFlags[safeNextRowIndexs[rnd.Next(safeNextRowIndexs.Count)]] = false;
        }

        // Set next Row hazards as previous Hazards
        SetPreviousRowState(nextRowHazardFlags, safeNextRowIndexs);

        return nextRowHazardFlags;
    }

    private HashSet<int> GenerateNextRowSafeIndexs()
    {
        var safeNextRowIndexs = new HashSet<int>();
        for (int i = 0; i < _previousRowState.Count; i++)
        {
            if (!_previousRowState[i].isHazard && _previousRowState[i].isValidPath)
            {
                if (!isLargeRow(_previousRowState.Count))
                {
                    safeNextRowIndexs.Add(i);
                    safeNextRowIndexs.Add(i+1);
                }
                else
                {
                    if (i < _previousRowState.Count - 1)
                    {
                        safeNextRowIndexs.Add(i);
                    }

                    if (i > 0)
                    {
                        safeNextRowIndexs.Add(i - 1);
                    }
                }
            }
        }

        return safeNextRowIndexs;
    }

    private void SetPreviousRowState(List<bool> nextRowHazardFlags, IEnumerable<int> safeNextRowIndexs)
    {
        var nextPreviousRowState = new List<PreviousBlockState>();
        for (int i = 0; i < nextRowHazardFlags.Count; i++)
        {
            if (safeNextRowIndexs.Contains(i))
            {
                if (!nextRowHazardFlags[i])
                {
                    nextPreviousRowState.Add(new PreviousBlockState { isHazard = false, isValidPath = true });
                    continue;
                }
            }
            nextPreviousRowState.Add(new PreviousBlockState { isHazard = nextRowHazardFlags[i], isValidPath = false });
        }
        _previousRowState = nextPreviousRowState;
    }

    public bool isLargeRow(int rowLength)
    {
        // Odd rows are always large
        return rowLength % 2 == 1;
    }

    public int GetNumberOfRowsSpawned()
    {
        return _rowsSpawned;
    }

    private Vector3 GenerateRandomVector3BeyondXValue(float x)
    {
        return new Vector3(x + Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10));
    }
}
