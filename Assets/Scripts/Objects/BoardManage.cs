//using System;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BoardManage : MonoBehaviour
{
    [SerializeField]
    private StackBlockShooter _stackBlockShooter;
    private GamePlayState _gamePlayState = GamePlayState.Idle;
    private StackBlock[,] stackBlocks;
    private int[] _shootAblePlaces = new int[ConstData.ROW_BLOCKS];
    private ShootPlaceHolder[] _shootPlaceHolders = new ShootPlaceHolder[ConstData.ROW_BLOCKS];
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

    public void GenerateNewBoard()
    {
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            for (int j = 0; j < ConstData.COL_BLOCKS; j++)
            {
                stackBlocks[i, j] = PoolManage.Instance.GetObject<StackBlock>().GetComponent<StackBlock>();
                stackBlocks[i, j].transform.parent = transform;

                // Set position the board 
                stackBlocks[i, j].transform.position = GetGridPosition(i, j);
                if (j == 0)
                {
                    continue;
                }
                
                //randomize block amount
                // Todo: Don't use random, use game play data
                stackBlocks[i, j].SpawnBlock(UnityEngine.Random.Range(ConstData.MIN_BLOCKS, ConstData.MAX_BLOCKS));

                //Randomize block color
                stackBlocks[i, j].ChangeColor(
                    (BlockColor)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(BlockColor)).Length)
                );
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
                else if (stackBlocks[i, j].GetTotalBlocks() == 0)
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

#region InGameAction 
    private void FinishProcess()
    {
        _processingActionNumber--;
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

            //Merge case
            MergeBlockCall(col, 1); //Merge after push default at row 1
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
        _processingActionNumber++;
        StartCoroutine(MergeBlocks(col, row));
    }

    public IEnumerator MergeBlocks(int col, int row)
    {
        bool isMerge = false;

        //Check surrounding blocks
        foreach (var item in ConstData.SUROUND_DIRECTION)
        {
            int newRow = row + item[0];
            int newCol = col + item[1];
            if (newCol < 0 ||
                newCol >= ConstData.ROW_BLOCKS ||
                newRow < 0 ||
                newRow >= ConstData.COL_BLOCKS ||
                isMerge)
            {
                continue;
            }

            // Merge action
            if (stackBlocks[newCol, newRow].GetTotalBlocks() > 0 &&
                stackBlocks[newCol, newRow].GetBlockColor() == stackBlocks[col, row].GetBlockColor())
            {
                isMerge = true;
                stackBlocks[col, row].MoveToOtherStack(stackBlocks[newCol, newRow], ConstData.INTENSE_CURVE, ConstData.MERGE_WAIT_TIME);
                yield return new WaitForSeconds(ConstData.MERGE_WAIT_TIME + ConstData.REST_WAIT_TIME);
                MergeBlockCall(newCol, newRow);
                break;
            }
        }

        _processingActionNumber--;
        if (!isMerge)
        {
            stackBlocks[col, row].GetPoint();
        }
    }

    //
    public void PushColumn()
    {
        int order = 0;
        for (int i = 0; i < ConstData.COL_BLOCKS; i++)
        {
            if (stackBlocks[_currentShoot, i].GetListObject<Block>().Count == 0)
            {
                break;
            }
            order = i;
        }
        for (int i = order; i >= 0; i--)
        {
            //Start 1 action
            _processingActionNumber += stackBlocks[_currentShoot, i].GetListObject<Block>().Count;
            
            if (i + 1 == ConstData.COL_BLOCKS)
            {
                //Animation
                stackBlocks[_currentShoot, i].MoveOutOfBoard(ConstData.BLOCK_SPEED, FinishProcess);
                //Data change
                stackBlocks[_currentShoot, i].DespawnBlock();
            }
            else
            {
                //Animation
                stackBlocks[_currentShoot, i].MoveToOtherStack(stackBlocks[_currentShoot, i + 1], ConstData.BLOCK_SPEED, FinishProcess);
                //Data change
                stackBlocks[_currentShoot, i + 1].AddBlock(stackBlocks[_currentShoot, i]);
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
        if (!_shootAblePlaces.Any(x => x >= 0))
        {
            return GamePlayState.Lose;
        }
        GamePlayState result = GamePlayState.Win;
        for (int i = 0; i < ConstData.ROW_BLOCKS; i++)
        {
            for (int j = 0; j < ConstData.COL_BLOCKS; j++)
            {
                if (stackBlocks[i, j].GetTotalBlocks() > 0)
                {
                    result = GamePlayState.Idle;
                }
            }
        }
        return _processingActionNumber > 0 ? GamePlayState.Processing : result;
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
        _stackBlockShooter.ReloadShooter();
    }
#endregion
}
