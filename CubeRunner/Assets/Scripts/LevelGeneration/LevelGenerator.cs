using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Blocks;
using Assets.Scripts.Player;
using Assets.Scriptss;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    private Queue<float> _distanceToSpawnNextWallQueue = new Queue<float>();
    private List<PreviousBlockState> _previousRowState = new List<PreviousBlockState>();
    private int _rowsSpawned = 0;

    private System.Random _rnd = new System.Random();
    private ColorMap _currentLevelColorMap;

    public float hazardPercentage = 0.1f;

    public GameObject wallBlockPrefab;
    public GameObject floorBlockPrefab;
    public GameObject holeBlockPrefab;
    public GameObject spikeBlockPrefab;

    public GameObject mainPlayer;

    //TODO REMOVE DEBUG FUNCTION

    

    public void SetHazardPercentage(Slider slider)
    {
        hazardPercentage = slider.value;
    }

    //END TODO


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
        _currentLevelColorMap = BlockColorMaps.SelectRandomColorMapping();
        mainPlayer.GetComponent<PlayerController>().SetColor(_currentLevelColorMap.PlayerMainColor);
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
        var floorRowWidth = isSmallRow ? GameConstants.initialRowOddWidth - 1 : GameConstants.initialRowOddWidth;

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
                if (Random.value < 0.70f)
                {
                    floorBlock = Instantiate(holeBlockPrefab, blockInitialPosition, Quaternion.Euler(0, 45, 0))
                        .GetComponent<HoleBlockController>();
                }else
                {
                    floorBlock = Instantiate(spikeBlockPrefab, blockInitialPosition, Quaternion.Euler(0, 45, 0))
                        .GetComponent<FloorSpikeBlockController>();
                }
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

            //TODO REMOVE DEBUG CODE
            //Set color for isreachableAsWell

            var isOffColorRow = rowNumber % 2 == 0;

            if (floorBlock.GetType() != typeof(HoleBlockController))
            {
                floorBlock.SetColor(_currentLevelColorMap.FloorMainColor, isOffColorRow, _previousRowState[i].isValidPath && GameConstants.highlightValidPath);
            }

            //TODO END REMOVE DEBUG CODE
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
                .8f,
                GameConstants.halfblockWidth);
            rightBlockFinalPosition = new Vector3(
                rowNumber * GameConstants.blockWidth / 2f,
                .8f,
                GameConstants.initialRowOddWidth * GameConstants.blockWidth + GameConstants.halfblockWidth);
        }
        else
        {
            leftBlockFinalPosition = new Vector3(
                rowNumber * GameConstants.halfblockWidth,
                .8f,
                0);
            rightBlockFinalPosition = new Vector3(
                rowNumber * GameConstants.halfblockWidth,
                .8f,
                GameConstants.blockWidth + GameConstants.initialRowOddWidth * GameConstants.blockWidth);
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
        Dictionary<int, bool> safeNextRowIndexs = new Dictionary<int, bool>();
        var nextRowLength = _previousRowState.Count == 0 ? GameConstants.initialRowOddWidth : isLargeRow(_previousRowState.Count) ? GameConstants.initialRowOddWidth - 1 : GameConstants.initialRowOddWidth;

        //For starter rows make them hazard free
        if (_rowsSpawned < GameConstants.rowLeadLength)
        {
            for (int i = 0; i < nextRowLength; i++)
            {
                nextRowHazardFlags.Add(false);
                safeNextRowIndexs.Add(i, i == 0);
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
        safeNextRowIndexs = GenerateNextReachableBlockIndexs();
        var foundSafePath = false;

        // Make sure valid path is not hazard
        foreach (var i in safeNextRowIndexs.Keys)
        {
            if (safeNextRowIndexs[i])
            {
                nextRowHazardFlags[i] = false;
            }
        }

        // Set next Row hazards as previous Hazards
        SetPreviousRowState(nextRowHazardFlags, safeNextRowIndexs);

        return nextRowHazardFlags;
    }

    // Safe index -> isValidPath
    private Dictionary<int, bool> GenerateNextReachableBlockIndexs()
    {
        var safeNextRowIndexs = new Dictionary<int, bool>();
        for (int i = 0; i < _previousRowState.Count; i++)
        {
            if (!_previousRowState[i].isHazard && _previousRowState[i].isReachablePath)
            {
                if (!isLargeRow(_previousRowState.Count))
                {
                    var rnd = _rnd.Next(10);
                    AddOrUpdateSafeNextRowDictionary(safeNextRowIndexs, i, _previousRowState[i].isValidPath && rnd < 5);
                    AddOrUpdateSafeNextRowDictionary(safeNextRowIndexs, i + 1, _previousRowState[i].isValidPath && rnd >= 5);
                }
                else
                {
                    
                    if (i < _previousRowState.Count - 1 && i > 0)
                    {
                        var rnd = _rnd.Next(10);
                        AddOrUpdateSafeNextRowDictionary(safeNextRowIndexs, i, _previousRowState[i].isValidPath && rnd < 5);
                        AddOrUpdateSafeNextRowDictionary(safeNextRowIndexs, i - 1, _previousRowState[i].isValidPath && rnd >= 5);
                    }
                    else
                    {
                        if (i < _previousRowState.Count - 1)
                        {
                            AddOrUpdateSafeNextRowDictionary(safeNextRowIndexs, i, _previousRowState[i].isValidPath);
                        }
                        if (i > 0)
                        {
                            AddOrUpdateSafeNextRowDictionary(safeNextRowIndexs, i - 1, _previousRowState[i].isValidPath);
                        }
                    }
                }
            }
        }

        return safeNextRowIndexs;
    }

    private void AddOrUpdateSafeNextRowDictionary(Dictionary<int, bool> safeNextRowIndexs, int key, bool value)
    {
        if (safeNextRowIndexs.ContainsKey(key))
        {
            safeNextRowIndexs[key] = safeNextRowIndexs[key] || value;
        }
        else
        {
            safeNextRowIndexs.Add(key, value);
        }
    }

    private void SetPreviousRowState(List<bool> nextRowHazardFlags, Dictionary<int, bool> safeNextRowIndexs)
    {
        var nextPreviousRowState = new List<PreviousBlockState>();
        for (int i = 0; i < nextRowHazardFlags.Count; i++)
        {
            if (safeNextRowIndexs.Keys.Contains(i))
            {
                if (!nextRowHazardFlags[i])
                {
                    nextPreviousRowState.Add(new PreviousBlockState { blockIndex = i, isHazard = false, isReachablePath = true, isValidPath = safeNextRowIndexs[i]});
                    continue;
                }
            }
            nextPreviousRowState.Add(new PreviousBlockState { blockIndex = i, isHazard = nextRowHazardFlags[i], isReachablePath = false, isValidPath = false});
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
