using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;

public class StackBlockShooter : AbstractStackBlock, IDragableObject
{
    private Vector3 _previousPos;
    private bool _isDragable = false;

    public delegate void ChangePosCallback();
    public delegate GamePlayState ShootStackCallback(float posX);
    public ChangePosCallback changePosCallback;
    public ShootStackCallback shootStackCallback;

    void Start()
    {
        _previousPos = transform.position;
    }

    void Update()
    {
        //On shooter base move
        if (transform.position != _previousPos)
        {
            for (int i = 0; i < blocks.Count; i++) 
            {
                blocks[i].GetComponent<Block>().MovePassOver(
                    new Vector3(transform.position.x, blocks[i].transform.position.y, blocks[i].transform.position.z),
                    ConstData.INTENSE_DISTANCE, 
                    ConstData.DEFAULT_SPEED * (blocks.Count - i)
                );
            }
            _previousPos = transform.position;
            changePosCallback?.Invoke();
        }
    }

    public void SetUpWithBoard(ChangePosCallback changePosCallback, ShootStackCallback shootStackCallback)
    {
        this.changePosCallback = changePosCallback;
        this.shootStackCallback = shootStackCallback;
    }

    public void ReloadShooter()
    {
        SpawnBlock(Random.Range(ConstData.MIN_BLOCKS, ConstData.MAX_BLOCKS));
        ColorType = (BlockColor)Random.Range(0, System.Enum.GetValues(typeof(BlockColor)).Length);
        _isDragable = true;
    }

    public bool IsDragable()
    {
        return _isDragable;
    }

    public override void DespawnBlock()
    {
        if (blocks.Count > 0)
        {
            // Remove the current block
            blocks = new List<GameObject>();
        }
    }

    public void ActionEndDrag()
    {
        //Todo: Check status of the game
        if (shootStackCallback?.Invoke(transform.position.x) != GamePlayState.Idle)
        {
            return;
        }
        _isDragable = false;
    }
}