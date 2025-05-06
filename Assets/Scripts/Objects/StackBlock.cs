using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class StackBlock : AbstractStackBlock
{
    [SerializeField]
    private TextMeshProUGUI _textCount;

    private int _layerLevel = 0;
    private int _order;
    private int _progressNumber = 0;
    private System.Action<BlockMoveState, int> _callBack;

#region Block
    public int GetNumberBlocks()
    {
        List<GameObject> listObj = GetListObject<Block>();
        int result = 0;
        for (int i = listObj.Count - 1; i >= 0; i--)
        {
            if (listObj[i].GetComponent<Block>().ColorType == ColorType)
            {
                result++;
            }
            else {
                break;
            }
        }
        return result;
    }

    protected override void OnAwake()
    {
        _textCount.transform.SetParent(GameObject.Find("CanvasWorld").transform);
    }

    public override void DespawnBlock(int amount = -1)
    {
        List<GameObject> tempList = GetListObject<Block>();
        if (amount > 0)
        {
            if (tempList.Count >= amount)
            {
                for (int i = 0; i < amount; i++)
                {
                    tempList.RemoveAt(tempList.Count - 1);
                }
            } 
        }
        if (tempList.Count > 0)
        {
            tempList.RemoveAll(x => true);
        }
    }

    /// <summary>
    /// Add more block from param
    /// </summary>
    /// <typeparam name="T">Abs of Stack</typeparam>
    /// <param name="block">Stack</param>
    public override void AddBlock<T>(T block)
    {
        List<GameObject> sendStack = block.GetListObject<Block>();
        List<GameObject> receiveStack = GetListObject<Block>();
        int loopCount = sendStack.Count;
        int totalAdd = 0;
        for (int i = 0; i < loopCount; i++)
        {
            if (ColorType != block.ColorType && receiveStack.Count != 0)
            {
                break;
            }
            receiveStack.Add(sendStack.Last());
            sendStack.RemoveAt(sendStack.Count - 1);
            totalAdd++;
        }
        // Pushed to this from other stack
        UpdateStackUI();

        //Change other Stack
        // block.DespawnBlock(totalAdd);
        block.UpdateStackUI();
    }

    public override void AddBlock<T>(T block, bool isMerge)
    {
        if (isMerge)
        {
            AddBlock(block);
            return;
        }
        GetListObject<Block>().AddRange(block.GetListObject<Block>());
        // ColorType = block.ColorType;
        
        // Pushed to this from other stack
        UpdateStackUI();

        //Change other Stack
        block.DespawnBlock();
        block.UpdateStackUI();
    }

    public override void OrderBlocks()
    {
        base.OrderBlocks();
        _textCount.text = GetTotalBlocks().ToString();
    }

    public void SetTransform(Vector3 location)
    {
        transform.position = location;
        _textCount.transform.position = location;
    }

    // Move for shoot or push
    public void MoveToOtherStack(StackBlock block, float speed, System.Action callback)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++) 
        {
            Vector3 tagetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * (block.GetTotalBlocks() + i);
            tempList[i].GetComponent<Block>().MoveStraight(tagetPos, speed, callback);
            tempList[i].transform.SetParent(block.transform);
        }
        // Change in the data
    }

    // Move for merge
    public void MoveToThisStack(StackBlock block, float intense, float speed, int order, System.Action<BlockMoveState, int> callBack)
    {
        BlockColor currentColor = block.ColorType;
        _callBack = callBack;
        _order = order;
        
        List<GameObject> tempList = block.GetListObject<Block>().AsEnumerable().Reverse().ToList();
        
        //Start move in this stack
        if (_progressNumber == 0)
        {
            callBack(BlockMoveState.StartLeave, order);
        }
        //Animation
        for (int i = 0; i < tempList.Count; i++) 
        {
            if (currentColor == tempList[i].GetComponent<Block>().ColorType)
            {
                _progressNumber++;
                float delay = ConstData.MERGE_WAIT_TIME * i;
                Vector3 tagetPos = transform.position + Vector3.up * ConstData.GASP_BLOCK * (GetTotalBlocks() + i);
                tempList[i].GetComponent<Block>().MoveCurve(
                    block,
                    transform,
                    tagetPos,
                    intense,
                    speed,
                    delay,
                    FinishBlockMove);
            }
            else
            {
                StartCoroutine(InvokeLastLeaveCallback(ConstData.MERGE_WAIT_TIME * i, order, callBack));
                break;
            }
            if (i == tempList.Count - 1)
            {
                StartCoroutine(InvokeLastLeaveCallback(ConstData.MERGE_WAIT_TIME * i, order, callBack));
            }
        }
        
        //Data
        AddBlock(block);
        
        //Layer
        block.SetStackBlockLayer(true);
        SetStackBlockLayer(true);
    }

    private IEnumerator InvokeLastLeaveCallback(float time, int order, System.Action<BlockMoveState, int> callBack)
    {
        yield return new WaitForSeconds(time);
        callBack(BlockMoveState.LastLeave, order);
    }

    private void FinishBlockMove(BlockMoveState state, StackBlock block)
    {
        if (state == BlockMoveState.MergeDone)
        {
            _progressNumber--;
            if (_progressNumber == 0)
            {
                // Set layer back to normal
                SetStackBlockLayer(false);
                block.SetStackBlockLayer(false);
                // Next action on board
                _callBack(BlockMoveState.MergeDone, _order);
            }
        }
    }

    //Last stack only
    public void MoveOutOfBoard(Vector3 pos, float speed, System.Action callBack)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++)
        {
            // Vector3 addPos = tempList[i].transform.position + Vector3.up * ConstData.UNIT_DISTANCE;
            GameObject tempBlock = tempList[i];
            tempBlock.GetComponent<Block>().MoveStraight(pos, speed, () => MoveOutOfBoardCallBack(tempBlock, callBack));
        }
    }

    public void MoveOutOfBoardCallBack(GameObject item, System.Action callBack)
    {
        item.SetActive(false);
        callBack();
    }

    //Get attribute

    public int GetTotalBlocks()
    {
        return GetListObject<Block>().Count;
    }

// Todo: Fix gain point condition
    public void GainPoint()
    {
        int blockCount = GetTotalBlocks();
        if (blockCount >= 10)
        {
            int keepBlock = blockCount % 10;
            UIControl.Instance.AddScore(blockCount - keepBlock);
            for (int i = keepBlock; i < blockCount; i++)
            {
                GetListObject<Block>()[i].SetActive(false);
            }
            GetListObject<Block>().RemoveRange(keepBlock, blockCount - keepBlock);

            UpdateStackUI();
        }
    }

    public override void UpdateStackUI()
    {
        int blockCount = GetTotalBlocks();
        _textCount.gameObject.SetActive(blockCount > 0);
        _textCount.text = blockCount.ToString();
    }

    public void SetStackBlockLayer(bool isMove) 
    {
        if (isMove)
        {   
            this.GetComponent<SortingGroup>().sortingOrder = _layerLevel + 2;
        }
        else
        {
            this.GetComponent<SortingGroup>().sortingOrder = _layerLevel;
        }
    }

    public void SetStackBlockLayer(int level)
    {
        this.GetComponent<SortingGroup>().sortingOrder = level;
        _layerLevel = level;
    }
#endregion

#region Special Obj
    public bool CheckContainObstacle()
    {
        return GetListObject<Obstacle>().Count > 0;
    }

    public void SetBlockLayer(int level)
    {
        foreach (GameObject item in GetListObject<Block>())
        {
            item.GetComponent<Block>().SetLayer(level);
        }
    }
#endregion
}
