using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class StackBlock : AbstractStackBlock
{
    [SerializeField]
    private TextMeshProUGUI _textCount;

#region Block
    protected override void OnAwake()
    {
        _textCount.transform.SetParent(GameObject.Find("CanvasWorld").transform);
    }

    public override void DespawnBlock()
    {
        List<GameObject> tempList = GetListObject<Block>();
        if (tempList.Count > 0)
        {
            tempList.RemoveAll(x => true);
        }
    }

    // Add more block from param
    public override void AddBlock<T>(T block)
    {
        // Pushed to this from other stack
        GetListObject<Block>().AddRange(block.GetListObject<Block>());
        ColorType = block.ColorType;
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

    public void MoveToOtherStack(StackBlock block, float speed, System.Action callback)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++) 
        {
            Vector3 tagetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * (block.GetTotalBlocks() + i);
            tempList[i].GetComponent<Block>().MoveStraight(tagetPos, speed, callback);
        }
        // Change in the data
        block.AddBlock(this);
    }

    public void MoveToOtherStack(StackBlock block, float intense, float time)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++) 
        {
            Vector3 tagetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * (block.GetTotalBlocks() + i);
            tempList[i].GetComponent<Block>().MoveCurve(tagetPos, intense, time);
        }
        block.AddBlock(this);
    }

    //Last stack only
    public void MoveOutOfBoard(float speed, System.Action callBack)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++)
        {
            Vector3 addPos = tempList[i].transform.position + Vector3.up * ConstData.UNIT_DISTANCE;
            GameObject tempBlock = tempList[i];
            tempBlock.GetComponent<Block>().MoveStraight(addPos, speed, () => MoveOutOfBoardCallBack(tempBlock, callBack));
        }
    }

    public void MoveOutOfBoardCallBack(GameObject item, System.Action callBack)
    {
        item.SetActive(false);
        callBack();
    }

    //Get attribute
    public void ChangeColor(BlockColor color) 
    {
        ColorType = color;
    } 

    public int GetTotalBlocks()
    {
        return GetListObject<Block>().Count;
    }

    public BlockColor GetBlockColor()
    {
        return ColorType;
    }

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
