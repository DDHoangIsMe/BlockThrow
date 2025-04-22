//using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

enum GamePlayState
{
    Idle,
    Push,
    Win,
    Lose,
    None
}

public class BoardManage : MonoBehaviour
{
    private GamePlayState gamePlayState = GamePlayState.Idle;
    private StackBlock[,] stackBlocks;
    private int[] shootAblePlaces = new int[ConstData.ROW_BLOCKS];

    void Awake()
    {
        stackBlocks = new StackBlock[ConstData.ROW_BLOCKS, ConstData.COL_BLOCKS];
        GenerateNewBoard();
    }

    public void GenerateNewBoard()
    {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            stackBlocks[i, 0] = new GameObject().AddComponent<StackBlock>();
        }
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            for (int j = 1; j < ConstData.COL_BLOCKS; j++)
            {
                stackBlocks[i, j] = new GameObject().AddComponent<StackBlock>();
                stackBlocks[i, j].transform.parent = transform;
                // Set position the board 
                stackBlocks[i, j].transform.position = new Vector3(
                    ((ConstData.ROW_BLOCKS - 1) / 2 - i) * ConstData.UNIT_DISTANCE, 
                    ((ConstData.COL_BLOCKS - 1) / 2 - j) * ConstData.UNIT_DISTANCE
                );
                //randomize block amount
                // Todo: Don't use random, use game play data
                stackBlocks[i, j].SpawnBlock(Random.Range(ConstData.MIN_BLOCKS, ConstData.MAX_BLOCKS));
                //Randomize block color
                stackBlocks[i, j].ChangeColor(
                    (BlockColor)Random.Range(0, System.Enum.GetValues(typeof(BlockColor)).Length)
                );
            }
        }
    }

    public void MarkShootablePlace(StackBlockShooter block)
    {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            for (int j = 1; j < ConstData.COL_BLOCKS; j++)
            {
                if (stackBlocks[i, j] == null)
                {
                    throw new System.Exception("Error: board out of range");
                }
                else if (stackBlocks[i, j].GetTotalBlocks() <= 0)
                {
                    continue;
                }
                else 
                {
                    shootAblePlaces[i] = j - 1; // choose the right below this block 
                    //Todo: Show animation shootable at place
                    Debug.Log("Shootable at: " + i + " , " + (j - 1));
                }
            }
        }
    }

    public void ShootAtPlace(StackBlockShooter block, int col)
    {
        stackBlocks[col, shootAblePlaces[col]].AddBlock(block);
        MergeBlockCall(col, shootAblePlaces[col]);
    }

    public void MergeBlockCall(int col, int row)
    {
        StartCoroutine(MergeBlocks(col, row, MergeBlockCall));
    }

    public IEnumerator MergeBlocks(int col, int row, System.Action<int, int> callback)
    {
        bool isContinue = false;
        //Check surrounding blocks
        for (int i = col - 1; i <= col + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                //Ignore the block itself and out of bounds
                if (
                    (i == col && j == row) ||
                    i < 0 ||
                    i >= ConstData.ROW_BLOCKS ||
                    j < 0 ||
                    j >= ConstData.COL_BLOCKS
                )
                {
                    continue;
                }
                //Merge
                if (
                    stackBlocks[i, j].GetTotalBlocks() > 0 && 
                    stackBlocks[i, j].GetBlockColor() == stackBlocks[col, row].GetBlockColor()
                )
                {
                    isContinue = true;
                    stackBlocks[col, row].MoveToOtherStack(stackBlocks[i, j]);
                    yield return new WaitForSeconds(ConstData.MERGE_WAIT_TIME);
                    callback(col, row);
                    break;
                }
            }
        }
        if (!isContinue)
        {
            PushColumn();
        }
    }

    public void PushColumn()
    {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++) 
        {
            if (stackBlocks[i, 0].GetTotalBlocks() > 0)
            {
                int numberPushedBlock = 0;
                for (int j = shootAblePlaces[i]; j < ConstData.COL_BLOCKS; i++)
                {
                    if (stackBlocks[i, j].GetTotalBlocks() == 0)
                    {
                        numberPushedBlock = j;
                        gamePlayState = GamePlayState.Push;
                        break;
                    }
                }
                if (gamePlayState == GamePlayState.Idle) 
                {
                    //Todo: End game
                    Debug.Log("Game Over!");
                }
                else 
                {
                    for (int j = numberPushedBlock - 1; j >= 0; j--)
                    {
                        stackBlocks[i, j].MoveToOtherStack(stackBlocks[i, j + 1]);
                    }
                    gamePlayState = GamePlayState.Idle;
                }
                break;
            }
        }
    }

    public void ClearBoard()
    {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            for (int j = 0; j < ConstData.COL_BLOCKS; j++)
            {
                stackBlocks[i, j].DespawnBlock();
            }
        }
    }
}
