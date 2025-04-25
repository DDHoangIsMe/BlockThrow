using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbstractStackBlock : MonoBehaviour, IStackable
{
    // Current color state of the block
    private BlockColor _colorType;
    // Get all IGameObject assigned class
    private Type[] types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IGameObject).IsAssignableFrom(t) && t.IsClass)
            .ToArray();

    private List<List<GameObject>> _allList = new List<List<GameObject>>();

    // Color Property
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

    //Method on change UI
    public abstract void UpdateStackUI();

    protected virtual void OnAwake()
    {
        //Act as Awake
    }

    //Create object block
    public virtual void SpawnBlock(int amount, BlockColor color) 
    {
        // create new blocks
        SpawnObject<Block>(amount);
        ColorType = color;
        OrderBlocks();
    }

    //Place blocks in places
    public virtual void OrderBlocks() {
        for (int i = 0; i < GetListObject<Block>().Count; i++)
        {
            //Todo: Animation ordering blocks
            GetListObject<Block>()[i].transform.position = transform.position + Vector3.up * i * ConstData.GASP_BLOCK;
        }
    }

    //Create object
    public void SpawnObject<T>(int amount = 1) where T : IGameObject
    {
        // Create obj
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = PoolManage.Instance.GetObject<T>();
            GetListObject<T>().Add(obj);
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
        foreach (GameObject item in tempList)
        {
            item.SetActive(false);
        }
        DespawnBlock();
    }
}
