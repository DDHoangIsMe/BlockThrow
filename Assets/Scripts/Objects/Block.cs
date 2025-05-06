using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IGameObject, IMoveWithPassOver, IMoveWithCurve, IMoveObject
{
    public Coroutine moveCoroutine;
    private SpriteRenderer _blockSprite;

    private Transform _currentParent;
    private Vector3 _targetPos;
    private BlockColor _colorType;
    private float _speed;

    //For straight movement
    private bool _isMovingStraight = false;

    //For pass over movement
    private bool _isMovingPass = false;

    //For curve movement
    private bool _isMovingCurve = false;
    private Vector3 _startPoint;
    private Vector3 _curvePos;
    private float _progress;
    private float _distance;

    public BlockColor ColorType
    {
        get { return _colorType; }
        protected set {     
            _colorType = value;
        }
    }


    void Awake()
    {
        _blockSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (moveCoroutine != null)
        {
            if (_isMovingPass || _isMovingStraight)
            {
                 transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);
            }
            if (_isMovingCurve)
            {
                float t = _progress / _distance;

                // Bezier Quadratic Formula
                Vector3 position =  Mathf.Pow(1 - t, 2) * _startPoint +
                                    2 * (1 - t) * t * _curvePos +
                                    Mathf.Pow(t, 2) * _targetPos;

                transform.position = position;

                _progress += _speed * Time.deltaTime;

                if (_progress >= _distance) _progress = _distance;

                // Set parent on Scene
                if (t > 0.5f && transform.parent != _currentParent)
                {
                    transform.SetParent(_currentParent);
                }
            }
        }
    }

    public void OnCreateObject()
    {
        // Initialize the block object here
        Debug.Log("Block created!");
    }

    public void SetBlockColor(BlockColor color)
    {
        ColorType = color;
        Sprite newSprite = Resources.Load<Sprite>(ConstData.BLOCK_COLOR_TEXTURE_PATH + color.ToString());
        if (newSprite != null)
        {
            _blockSprite.sprite = newSprite;
        }
        else
        {
            // Testing zone
            Debug.Log("Sprite input is: "+ color.ToString());
        }
    }

    public void SetLayer(int level) 
    {
        // _blockSprite.sortingOrder = level;
    }

    public void DeactiveCoroutine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
            _isMovingCurve = false;
            _isMovingPass = false;
            _isMovingStraight = false;
            SetLayer(ConstData.BLOCK_LAYER);
        }
    }

    /// <summary>
    /// Make block move straight to target position
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="speed"></param>
    public void MoveStraight(Vector3 targetPos, float speed, System.Action callback)
    {
        DeactiveCoroutine();
        SetLayer(ConstData.MOVE_BLOCK_LAYER);
        _isMovingStraight = true;
        moveCoroutine = StartCoroutine(MoveStraightCoroutine(targetPos, speed, callback));
    }

    private IEnumerator MoveStraightCoroutine(Vector3 targetPos, float speed, System.Action callback)
    {
        this._speed = speed;
        this._targetPos = targetPos;
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < ConstData.OFF_SET);
        transform.position = targetPos;
        callback();
        DeactiveCoroutine();
    }
  
    /// <summary>
    /// Make block move with pass over path
    /// </summary>
    /// <param name="targetPos">End point</param>
    /// <param name="intense">Extra path</param>
    /// <param name="speed">Move's speed</param>
    public void MovePassOver(Vector3 targetPos, float intense, float speed)
    {
        DeactiveCoroutine();
        moveCoroutine = StartCoroutine(MovePassOverCoroutine(targetPos, intense, speed));
        _isMovingPass = true;
    }

    private IEnumerator MovePassOverCoroutine(Vector3 targetPos, float intense, float speed)
    {
        //Setup move for block
        _speed = speed + Vector3.Distance(transform.position, targetPos) * ConstData.BLOCK_SPEED;
        _targetPos = targetPos + (targetPos - transform.position) * intense;

        //Wait till block move
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _targetPos) < ConstData.OFF_SET);
        _targetPos = targetPos;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, _targetPos) < ConstData.OFF_SET);
        transform.position = targetPos;

        DeactiveCoroutine();
    }
    
    /// <summary>
    /// Make block move with curve path
    /// </summary>
    /// <param name="targetPos">End point</param>
    /// <param name="intense">Curve path</param>
    /// <param name="duration">Time animation</param>
    /// <param name="callBack">Trigger</param>
    public void MoveCurve(StackBlock senderStack, Transform parent, Vector3 targetPos, float intense, float speed, float delay, System.Action<BlockMoveState, StackBlock> callBack)
    {
        DeactiveCoroutine();
        _currentParent = parent;
        SetLayer(ConstData.MOVE_BLOCK_LAYER); //Todo: change SetLayer from block to all stack
        _progress = 0;
        moveCoroutine = StartCoroutine(MoveToCoroutine(senderStack, targetPos, intense, speed, callBack));
        Invoke(nameof(SetMovingCurve), delay);
    }

    private void SetMovingCurve()
    {
        _isMovingCurve = true;
    }

    private IEnumerator MoveToCoroutine(StackBlock senderStack, Vector3 targetPos, float intense, float speed, System.Action<BlockMoveState, StackBlock> callBack)
    {
        _targetPos = targetPos;
        _startPoint = transform.position;
        _distance = Vector3.Distance(_startPoint, _targetPos);
        _speed = speed;
        Vector3 normDirection = (targetPos - transform.position).normalized;
        if (Mathf.Sign(Vector3.Dot(normDirection, Vector3.right)) > 0)
        {
            normDirection.y = -normDirection.y; 
        }
        else
        {
            normDirection.x = -normDirection.x;
        }
        _curvePos = (targetPos + transform.position) / 2 + new Vector3(normDirection.y, normDirection.x, 0) * intense;
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _targetPos) < ConstData.OFF_SET);
        transform.position = targetPos;
        callBack(BlockMoveState.MergeDone, senderStack);
        _currentParent = null;
        DeactiveCoroutine();
    }
}
