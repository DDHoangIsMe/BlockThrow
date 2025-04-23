using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IGameObject, IMoveWithPassOver, IMoveWithCurve, IMoveObject
{
    public Coroutine moveCoroutine;
    private SpriteRenderer blockSprite;

    private Vector3 targetPos;
    private float speed;

    //For straight movement
    private bool isMovingStraight = false;

    //For pass over movement
    private bool isMovingPass = false;

    //For curve movement
    private bool isMovingCurve = false;
    private Vector3 startPoint;
    private Vector3 curvePos;
    private float duration;
    private float elapsedTime;


    void Awake()
    {
        blockSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (moveCoroutine != null)
        {
            if (isMovingPass || isMovingStraight)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }
            if (isMovingCurve)
            {
                if (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / duration;

                    // Quadratic Bezier formula
                    Vector3 position = Mathf.Pow(1 - t, 2) * startPoint +
                                    2 * (1 - t) * t * curvePos +
                                    Mathf.Pow(t, 2) * targetPos;

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
                blockSprite.color = Color.red;
                break;
            case BlockColor.Green:
                blockSprite.color = Color.green;
                break;
            case BlockColor.Blue:
                blockSprite.color = Color.blue;
                break;
            case BlockColor.Yellow:
                blockSprite.color = Color.yellow;
                break;
            case BlockColor.Purple:
                blockSprite.color = new Color(0.5f, 0, 0.5f); // Purple color RGB
                break;
        }
    }

    private void DeactiveMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
            isMovingCurve = false;
            isMovingPass = false;
            isMovingStraight = false;
        }
    }

    /// <summary>
    /// Make block move straight to target position
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="speed"></param>
    public void MoveStraight(Vector3 targetPos, float speed)
    {
        DeactiveMove();
        isMovingStraight = true;
        moveCoroutine = StartCoroutine(MoveStraightCoroutine(targetPos, speed));
    }

    private IEnumerator MoveStraightCoroutine(Vector3 targetPos, float speed)
    {
        this.speed = speed;
        this.targetPos = targetPos;
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < ConstData.OFF_SET);
        DeactiveMove();
    }
  
    /// <summary>
    /// Make block move with pass over path
    /// </summary>
    /// <param name="targetPos">End point</param>
    /// <param name="intense">Extra path</param>
    /// <param name="speed">Move's speed</param>
    public void MovePassOver(Vector3 targetPos, float intense, float speed)
    {
        DeactiveMove();
        isMovingPass = true;
        moveCoroutine = StartCoroutine(MovePassOverCoroutine(targetPos, intense, speed));
    }

    private IEnumerator MovePassOverCoroutine(Vector3 targetPos, float intense, float speed)
    {
        this.speed = speed + Vector3.Distance(transform.position, targetPos) * ConstData.BLOCK_SPEED;
        this.targetPos = targetPos + (targetPos - transform.position) * intense;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, this.targetPos) < ConstData.OFF_SET);
        this.targetPos = targetPos;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, this.targetPos) < ConstData.OFF_SET);
        DeactiveMove();
    }
    
    /// <summary>
    /// Make block move with curve path
    /// </summary>
    /// <param name="targetPos">End point</param>
    /// <param name="intense">Curve path</param>
    /// <param name="duration">Time animation</param>
    /// <param name="isAnimation">Trigger</param>
    public void MoveTo(Vector3 targetPos, float intense, float duration = 0, bool isAnimation = true)
    {
        DeactiveMove();
        isMovingCurve = true;
        elapsedTime = 0;
        moveCoroutine = StartCoroutine(MoveToCoroutine(targetPos, intense, duration, isAnimation));
    }
    private IEnumerator MoveToCoroutine(Vector3 targetPos, float intense, float duration, bool isAnimation)
    {
        this.duration = isAnimation ? duration : 0;
        this.targetPos = targetPos;
        this.startPoint = transform.position;
        Vector3 normDirection = (targetPos - transform.position).normalized;
        if (Mathf.Sign(Vector3.Dot(normDirection, Vector3.right)) > 0)
        {
            normDirection.y = -normDirection.y; 
        }
        else
        {
            normDirection.x = -normDirection.x;
        }
        curvePos = (targetPos + transform.position) / 2 + new Vector3(normDirection.y, normDirection.x, 0) * intense;
        yield return new WaitForSeconds(duration);
        DeactiveMove();
    }
}
