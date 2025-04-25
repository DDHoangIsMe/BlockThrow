using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbstractStackBlock : MonoBehaviour, IStackable
{
    // Current color state of the block
    private BlockColor _colorType;
    private Type[] types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IGameObject).IsAssignableFrom(t) && t.IsClass)
            .ToArray();
    public BlockColor ColorType
    {
        get { return _colorType; }
        protected set { 
            _colorType = value; 
            foreach (var block in GetListObject<Block>())
            {
                block.GetComponent<Block>().SetBlockColor(_colorType); // Set the color of each block
            }
        }
    }
    
    private List<List<GameObject>> _allList = new List<List<GameObject>>();

    void Awake()
    {
        foreach (Type item in types) 
        {
            _allList.Add(new List<GameObject>());
        }
        OnAwake();
    }

    //Method leave all the block out of this Stack
    public abstract void DespawnBlock();

    //Method logicaly add more block from other stack
    public abstract void AddBlock<T>(T otherStack) where T : AbstractStackBlock;

    protected virtual void OnAwake()
    {
        //Act as Awake
    }

    //Create object block
    public virtual void SpawnBlock(int amount) 
    {
        // create new blocks
        for (int i = 0; i < amount; i++)
        {
            GameObject block = PoolManage.Instance.GetObject<Block>();
            // block.transform.parent = this.transform;
            GetListObject<Block>().Add(block);
        }
        OrderBlocks();
    }

    public virtual void SpawnObject<T>() where T : IGameObject
    {
        
    }

    //Place blocks in places
    public virtual void OrderBlocks() {
        for (int i = 0; i < GetListObject<Block>().Count; i++)
        {
            //Todo: Animation ordering blocks
            GetListObject<Block>()[i].transform.position = transform.position + Vector3.up * i * ConstData.GASP_BLOCK;
        }
    }

    public List<GameObject> GetListObject<T>() where T : IGameObject
    {
        return _allList[Array.IndexOf(types, typeof(T))];
    }

    //Method destroy all the block in this Stack
    public void DestroyBlock()
    {
        List<GameObject> tempList = GetListObject<Block>();
        Debug.Log(tempList.Count);
        foreach (GameObject item in tempList)
        {
            item.SetActive(false);
        }
        DespawnBlock();
    }
}
