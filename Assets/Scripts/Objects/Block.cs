using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IGameObject, IMoveWithPassOver, IMoveWithCurve, IMoveObject
{
    public Coroutine moveCoroutine;
    private SpriteRenderer _blockSprite;

    private Vector3 _targetPos;
    private float _speed;

    //For straight movement
    private bool _isMovingStraight = false;

    //For pass over movement
    private bool _isMovingPass = false;

    //For curve movement
    private bool _isMovingCurve = false;
    private Vector3 _startPoint;
    private Vector3 _curvePos;
    private float _duration;
    private float _elapsedTime;


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
                if (_elapsedTime < _duration)
                {
                    _elapsedTime += Time.deltaTime;
                    float t = _elapsedTime / _duration;

                    // Quadratic Bezier formula
                    Vector3 position = Mathf.Pow(1 - t, 2) * _startPoint +
                                    2 * (1 - t) * t * _curvePos +
                                    Mathf.Pow(t, 2) * _targetPos;

                    transform.position = position;
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
        // Set the block color based on the BlockColor enum
        switch (color)
        {
            case BlockColor.Red:
                _blockSprite.color = Color.red;
                break;
            case BlockColor.Green:
                _blockSprite.color = Color.green;
                break;
            case BlockColor.Blue:
                _blockSprite.color = Color.blue;
                break;
            case BlockColor.Yellow:
                _blockSprite.color = Color.yellow;
                break;
            case BlockColor.Purple:
                _blockSprite.color = new Color(0.5f, 0, 0.5f); // Purple color RGB
                break;
        }
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
        _isMovingPass = true;
        moveCoroutine = StartCoroutine(MovePassOverCoroutine(targetPos, intense, speed));
    }

    private IEnumerator MovePassOverCoroutine(Vector3 targetPos, float intense, float speed)
    {
        this._speed = speed + Vector3.Distance(transform.position, targetPos) * ConstData.BLOCK_SPEED;
        this._targetPos = targetPos + (targetPos - transform.position) * intense;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, this._targetPos) < ConstData.OFF_SET);
        this._targetPos = targetPos;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, this._targetPos) < ConstData.OFF_SET);
        transform.position = targetPos;
        DeactiveCoroutine();
    }
    
    /// <summary>
    /// Make block move with curve path
    /// </summary>
    /// <param name="targetPos">End point</param>
    /// <param name="intense">Curve path</param>
    /// <param name="duration">Time animation</param>
    /// <param name="isAnimation">Trigger</param>
    public void MoveCurve(Vector3 targetPos, float intense, float duration = 0, bool isAnimation = true)
    {
        DeactiveCoroutine();
        _isMovingCurve = true;
        _elapsedTime = 0;
        moveCoroutine = StartCoroutine(MoveToCoroutine(targetPos, intense, duration, isAnimation));
    }
    private IEnumerator MoveToCoroutine(Vector3 targetPos, float intense, float duration, bool isAnimation)
    {
        this._duration = isAnimation ? duration : 0;
        this._targetPos = targetPos;
        this._startPoint = transform.position;
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
        yield return new WaitForSeconds(duration);
        transform.position = targetPos;
        DeactiveCoroutine();
    }
}
