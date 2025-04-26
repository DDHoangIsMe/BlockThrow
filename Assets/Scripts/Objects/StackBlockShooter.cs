using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using TMPro;

public class StackBlockShooter : AbstractStackBlock, IDragableObject
{
    [SerializeField]
    private TextMeshProUGUI _textCount;
    private Vector3 _previousPos;
    private bool _isDragable = false;

    public delegate void ChangePosCallback();
    public delegate GamePlayState ShootStackCallback(float posX);
    public ChangePosCallback changePosCallback;
    public ShootStackCallback shootStackCallback;

    protected override void OnAwake()
    {
        if (_textCount == null)
        {
            _textCount = new GameObject().AddComponent<TextMeshProUGUI>();
            _textCount.gameObject.AddComponent<CanvasRenderer>();
            _textCount.transform.SetParent(GameObject.Find("CanvasWorld").transform);
        }
    }

    void Start()
    {
        _previousPos = transform.position;
    }

    void Update()
    {
        //On shooter base move
        if (transform.position != _previousPos)
        {
            for (int i = 0; i < GetListObject<Block>().Count; i++) 
            {
                GetListObject<Block>()[i].GetComponent<Block>().MovePassOver(
                    new Vector3(transform.position.x, GetListObject<Block>()[i].transform.position.y, GetListObject<Block>()[i].transform.position.z),
                    ConstData.INTENSE_DISTANCE, 
                    ConstData.DEFAULT_SPEED * (GetListObject<Block>().Count - i)
                );
            }
            _textCount.transform.position = transform.position;
            _previousPos = transform.position;
            changePosCallback?.Invoke();
        }
#region Dev_Only
    #if UNITY_EDITOR
        //Testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadShooter(1, BlockColor.Red);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            ReloadShooter(1, BlockColor.Blue);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            ReloadShooter(1, BlockColor.Yellow);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            ReloadShooter(1, BlockColor.Green);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            ReloadShooter(1, BlockColor.Purple);
        }
    #endif
#endregion
#region Dev_Only
    #if UNITY_EDITOR
        //Testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadShooter(1, BlockColor.Red);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            ReloadShooter(1, BlockColor.Blue);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            ReloadShooter(1, BlockColor.Yellow);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            ReloadShooter(1, BlockColor.Green);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            ReloadShooter(1, BlockColor.Purple);
        }
    #endif
#endregion
    }

    public void SetUpWithBoard(ChangePosCallback changePosCallback, ShootStackCallback shootStackCallback)
    {
        this.changePosCallback = changePosCallback;
        this.shootStackCallback = shootStackCallback;
    }

    public override void AddBlock<T>(T block)
    {
        GetListObject<Block>().AddRange(block.GetListObject<Block>());
        ColorType = block.ColorType;
        block.DespawnBlock();
    }

    public override void UpdateStackUI()
    {
        if (_textCount != null)
        {
            _textCount.text = GetListObject<Block>().Count.ToString();
            _textCount.gameObject.SetActive(_isDragable);
        }
    }

    //Shoot block to other stack
    public void MoveToOtherStack<T>(T block, float speed, System.Action callBack) where T : AbstractStackBlock
    {
        //Use move straight animation
        for (int i = 0; i < GetListObject<Block>().Count; i++)
        {
            Vector3 targetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * i;
            GetListObject<Block>()[i].GetComponent<Block>().MoveStraight(targetPos, speed, callBack);
        }
        //Add in data
        block.AddBlock(this);
    }

    //Set this shooter new Block Stack
    public void ReloadShooter(int amount, BlockColor color)
    {
        DestroyBlock();
        SpawnBlock(amount, color);
        _isDragable = true;
        UpdateStackUI();
        // SpawnObject<Block>(amount);
        // ColorType = color;
    }

    public void ReloadShooter(int amount)
    {
        BlockColor color = (BlockColor)Random.Range(0, System.Enum.GetValues(typeof(BlockColor)).Length);
        ReloadShooter(amount, color);
    }

    public void ReloadShooter(BlockColor color)
    {
        int amount = Random.Range(ConstData.MIN_BLOCKS, ConstData.MAX_BLOCKS);
        ReloadShooter(amount, color);
    }

    public void ReloadShooter()
    {
        int amount = Random.Range(ConstData.MIN_BLOCKS, ConstData.MAX_BLOCKS);
        ReloadShooter(amount);
    }


    //Drag ability
    public bool IsDragable()
    {
        return _isDragable;
    }

    public void ActionEndDrag()
    {
        //Todo: Check status of the game
        if (shootStackCallback?.Invoke(transform.position.x) != GamePlayState.Idle)
        {
            return;
        }
        _isDragable = false;
        UpdateStackUI();
    }

    public override void DespawnBlock()
    {
        if (GetListObject<Block>().Count > 0)
        {
            // Remove the current block
            GetListObject<Block>().RemoveAll(x => true);
        }
    }
}