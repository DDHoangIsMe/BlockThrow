//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BoardManage : MonoBehaviour
{
    [SerializeField]
    private StackBlockShooter _stackBlockShooter;
    private GamePlayState _gamePlayState = GamePlayState.Idle;
    private StackBlock[,] stackBlocks;
    private int[] _shootAblePlaces = new int[ConstData.ROW_BLOCKS];
    private ShootPlaceHolder[] _shootPlaceHolders = new ShootPlaceHolder[ConstData.ROW_BLOCKS];

    
    private List<bool> _allowNextPath;
    private bool _allowNextMerge;
    private int _previousShootOrder = ConstData.NEGATIVE_ONE;
    private int _processingActionNumber = 0;

    private int _currentShoot
    {
        get
        {
            return GetShootPos();
        }
    }

    void Awake()
    {
        stackBlocks = new StackBlock[ConstData.ROW_BLOCKS, ConstData.COL_BLOCKS];

        //Board setup
        GenerateNewBoard();
        GenerateShootPlaceHolder();
        MarkShootablePlace();
        MarkPlace();
    }

    void Start()
    {
        //Shooter setup
        if (_stackBlockShooter == null)
        {
            _stackBlockShooter = transform.parent.Find(typeof(StackBlockShooter).Name).GetComponent<StackBlockShooter>();
        }
        _stackBlockShooter.SetUpWithBoard(OnShooterChangePos, OnShooterShootStack);
        ReloadShoot();
    }
#region BoardManage 
    public void GenerateNewBoard()
    {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            for (int j = 0; j < ConstData.COL_BLOCKS; j++)
            {
                stackBlocks[i, j] = PoolManage.Instance.GetObject<StackBlock>().GetComponent<StackBlock>();
                stackBlocks[i, j].transform.parent = transform;

                // Set position the board 
                stackBlocks[i, j].SetTransform(GetGridPosition(i, j));
                stackBlocks[i, j].SetStackBlockLayer(ConstData.COL_BLOCKS - j);
                
                if (j == 0)
                {
                    continue;
                }
#if UNITY_EDITOR
                //Todo: Remove this testzone
                if (j == 2 && i % 2 == 0)
                {
                    stackBlocks[i, j].SpawnObject<Obstacle>();
                    stackBlocks[i, j].GetListObject<Obstacle>()[0].transform.position = stackBlocks[i, j].transform.position;
                    continue;
                }
#endif
                //randomize block amount
                // Todo: Don't use random, use game play data
                int amount = UnityEngine.Random.Range(ConstData.MIN_BLOCKS, ConstData.MAX_BLOCKS);

                //Randomize block color
                BlockColor color = (BlockColor)UnityEngine.Random.Range(1, System.Enum.GetValues(typeof(BlockColor)).Length);
                
                stackBlocks[i, j].SpawnBlock(amount, color);
            }
        }
    }

    private void GenerateShootPlaceHolder() {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++) 
        {
            _shootPlaceHolders[i] = PoolManage.Instance.GetObject<ShootPlaceHolder>().GetComponent<ShootPlaceHolder>();
        }
    }

    public void MarkShootablePlace()
    {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            for (int j = 0; j < ConstData.COL_BLOCKS; j++)
            {
                if (stackBlocks[i, j] == null)
                {
                    //Todo: execute error 
                    throw new System.Exception("Error: board out of range");
                }
                else if(stackBlocks[i, j].GetTotalBlocks() == 0 && 
                        !stackBlocks[i, j].CheckContainObstacle())
                {
                    _shootAblePlaces[i] = j;
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void MarkPlace(bool trigger = true) {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            if (trigger)
            {
                _shootPlaceHolders[i].HideObject(false);
                _shootPlaceHolders[i].transform.position = GetGridPosition(i, _shootAblePlaces[i]);
            }
            else 
            {
                _shootPlaceHolders[i].HideObject();
            }
        }
    }

    private void HandlerGame()
    {
        //Todo: Handle case Win/Lose
        switch (_gamePlayState)
        {
            case GamePlayState.Idle:
                MarkPlace();
                ReloadShoot();
            break;
            case GamePlayState.Win:
            break;
            case GamePlayState.Lose:
            break;
            case GamePlayState.Processing:
                MarkPlace(false);
                StartCoroutine(ShootAtPlace());
            break;
            default: 
            break;
        }
    }
#endregion

#region InGameAction 
    private void FinishProcess()
    {
        _processingActionNumber--;
    }
    
    private void FinishProcess(BlockMoveState triggerProcess, int order)
    {
        // Update 1 process
        switch (triggerProcess)
        {
            case BlockMoveState.StartLeave:
                _processingActionNumber++;
            break;
            case BlockMoveState.LastLeave:
                if (order + 1 == _allowNextPath.Count)
                {
                    _allowNextMerge = true;
                }
            break;
            case BlockMoveState.AtPeak:
                //Handle at Stack
            break;
            case BlockMoveState.MergeDone:
                _allowNextPath[order] = true;
                _processingActionNumber--;
            break;
            case BlockMoveState.AllDone:
                _processingActionNumber--;
            break;
        }
    }

    public IEnumerator ShootAtPlace()
    {
        //From shooter to board
        int col = _currentShoot;
        ShootToBoard();
        yield return new WaitUntil(() => _processingActionNumber == 0);

        //Fix position
        stackBlocks[col, _shootAblePlaces[col]].OrderBlocks();

        //Merge case
        MergeBlockCall(col, _shootAblePlaces[col]);
        yield return new WaitUntil(() => _processingActionNumber == 0);

        //Push case
        if (_shootAblePlaces[col] == 0 && stackBlocks[col, 0].GetListObject<Block>().Count > 0)
        {
            PushColumn();
            yield return new WaitUntil(() => _processingActionNumber == 0);
        }

        //Next action
        _gamePlayState = CheckClearBoard();
        HandlerGame();
    }

    private void ShootToBoard()
    {
        _processingActionNumber += _stackBlockShooter.GetListObject<Block>().Count;
        StackBlock tempStack = stackBlocks[_currentShoot, _shootAblePlaces[_currentShoot]];
        _stackBlockShooter.MoveToOtherStack(tempStack, ConstData.BLOCK_SPEED, FinishProcess);
    }

    /// <summary>
    /// Recursive Function find all posibility merging
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    public void MergeBlockCall(int col, int row)
    {
        BlockColor previous = BlockColor.None;
        List<HashSet<(int, int)>> pathList = new List<HashSet<(int, int)>>();
        List<GameObject> blockList = stackBlocks[col, row].GetListObject<Block>();
        //Check all block in current stack and get it's path
        for (int i = blockList.Count - 1; i >= 0; i--)
        {
            if (blockList[i].GetComponent<Block>().ColorType == previous)
            {
                continue;
            }
            //Add new Merge Path
            previous = blockList[i].GetComponent<Block>().ColorType;
            HashSet<(int, int)> passed = new HashSet<(int, int)>();
            foreach (HashSet<(int, int)> item in pathList)
            {     
                passed.UnionWith(item);
            }
            HashSet<(int, int)> path = FindPath.FindBestPath(stackBlocks, (col, row), pathList.LastOrDefault(), previous);
            if (path.Count > 1)
            {
                pathList.Add(path);
            }
            else
            {
                break;
            }
        }
        
        //Use find path merge
        StartCoroutine(MergeBlocksMultiPath(pathList));
    }

    private IEnumerator MergeBlocksMultiPath(List<HashSet<(int, int)>> pathList)
    {
        _allowNextPath = new List<bool>();
        _allowNextMerge = false;
        //Loop to get HashSet
        for (int i = 0; i < pathList.Count; i++)
        {
            _allowNextPath.Add(false);
            _allowNextMerge = false;
            List<(int, int)> path = pathList[i].ToList();

            //Get each step move Stack to Stack
            StartCoroutine(MergeBlocksSinglePath(path, i));
            yield return new WaitUntil(() => _allowNextMerge);
        }
    }

    private IEnumerator MergeBlocksSinglePath(List<(int, int)> path, int order)
    {
        FinishProcess(BlockMoveState.StartLeave, order);
        for (int i = 1; i < path.Count; i++)
        {
            stackBlocks[path[i].Item1, path[i].Item2].MoveToThisStack(
                stackBlocks[path[i - 1].Item1, path[i - 1].Item2],
                ConstData.INTENSE_CURVE,
                ConstData.BLOCK_SPEED,
                order,
                FinishProcess);  //If this is the shooter place
                
            //Wait till all block with same color leave the current stack
            yield return new WaitUntil(() => _allowNextPath[order]);
            //Ready for next color
            yield return new WaitForSeconds(ConstData.REST_WAIT_TIME);
            _allowNextPath[order] = false;
        }
        FinishProcess(BlockMoveState.AllDone, order);
        
        //Part check if stack > 10 block
        GainBlockInStack(stackBlocks[path[path.Count - 1].Item1, path[path.Count - 1].Item2]);
    }

    private void GainBlockInStack(StackBlock block)
    {
        BlockColor color = block.ColorType;
    }

    public void PushColumn()
    {
        int order = 0;
        //Looking for empty Stack
        for (int i = 0; i < ConstData.COL_BLOCKS; i++)
        {
            if (stackBlocks[_currentShoot, i].GetTotalBlocks() == 0 &&
                !stackBlocks[_currentShoot, i].CheckContainObstacle())
            {
                break;
            }
            order = i;
        }

        //Move stacks up
        for (int i = order; i >= 0; i--)
        {
            StackBlock tempStackOrg = stackBlocks[_currentShoot, i];
            if (tempStackOrg.CheckContainObstacle())
            {
                continue;
            }
            //Start action process
            _processingActionNumber += tempStackOrg.GetTotalBlocks();
            
            StackBlock tempStackTarget = GetStackToPush(_currentShoot, i);
            if (tempStackTarget == null)
            {
                Vector3 endPos = new Vector3(
                    tempStackOrg.transform.position.x, 
                    (float)(ConstData.ROW_BLOCKS + 1) * ConstData.UNIT_DISTANCE / 2);
                //Animation
                tempStackOrg.MoveOutOfBoard(endPos, ConstData.BLOCK_SPEED, FinishProcess);
                //Data change
                tempStackOrg.DespawnBlock();
            }
            else
            {
                //Animation
                tempStackOrg.MoveToOtherStack(tempStackTarget, ConstData.BLOCK_SPEED, FinishProcess);
                //Data change
                tempStackTarget.AddBlock(tempStackOrg, false);
            }
        }
    }
#endregion

#region GridBoard
    //Grid on Board
    Vector3 GetGridPosition(int x, int y) =>
        new Vector3(
            (x - (float)(ConstData.ROW_BLOCKS - 1) / 2) * ConstData.UNIT_DISTANCE,
            (y - (float)(ConstData.COL_BLOCKS - 1) / 2) * ConstData.UNIT_DISTANCE
        );

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

    public GamePlayState CheckClearBoard()
    {
        MarkShootablePlace();
        // if (!_shootAblePlaces.Any(x => x >= 0))
        // {
        //     return GamePlayState.Lose;
        // }
        GamePlayState result = GamePlayState.Idle;
        // for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        // {
        //     for (int j = 0; j < ConstData.COL_BLOCKS; j++)
        //     {
        //         if (stackBlocks[i, j].GetTotalBlocks() > 0)
        //         {
        //             result = GamePlayState.Idle;
        //         }
        //     }
        // }
        return _processingActionNumber > 0 ? GamePlayState.Processing : result;
    }

    private StackBlock GetStackToPush(int row, int col)
    {
        //Check higher stack
        col++;
        for (int i = col; i < ConstData.COL_BLOCKS; i++)
        {
            if (!stackBlocks[row, i].CheckContainObstacle())
            {
                return stackBlocks[row, i];
            }
        }
        return null;
    }
#endregion

#region ShooterZone
    private int GetShootPos()
    {
        //Get Column
        int order = (int)Math.Round(_stackBlockShooter.transform.position.x/ ConstData.UNIT_DISTANCE) + (ConstData.ROW_BLOCKS - 1) / 2; //Half of row number
        return Mathf.Clamp(order, 0, ConstData.ROW_BLOCKS - 1);
    }

    private void OnShooterChangePos()
    {
        // Update current Shooter
        if (_currentShoot != _previousShootOrder && _gamePlayState == GamePlayState.Idle)
        {
            _shootPlaceHolders[_currentShoot].ChangeScale();
            _previousShootOrder = _currentShoot;
        }
    }

    private GamePlayState OnShooterShootStack(float posX)
    {
        //Todo: Remove this case
        if (_shootAblePlaces[_currentShoot] == ConstData.NEGATIVE_ONE)
        {
            return GamePlayState.None;
        }
        else if (_gamePlayState == GamePlayState.Idle)
        {
            _gamePlayState = GamePlayState.Processing;
            HandlerGame();
            return GamePlayState.Idle;
        }
        // Get the shooting position
        return _gamePlayState;
    }

    private void ReloadShoot()
    {
        _stackBlockShooter.ReloadShooter((float)2);
    }
#endregion
}
