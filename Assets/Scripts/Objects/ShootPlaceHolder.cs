using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPlaceHolder : MonoBehaviour, IGameObject, TweenHolder
{
    private Coroutine _tweenCoroutine;
    private Vector3 _targetScale;
    private Vector3 _originScale;

    void Awake()
    {
        OnCreateObject();
    }

    void LateUpdate()
    {
        if (_tweenCoroutine != null)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * ConstData.TWEEN_SCALE_SPEED);
        }
    }

    public void OnCreateObject()
    {
        // Init set up
        _originScale = transform.localScale;
    }

    public void HideObject(bool isHidden = true)
    {
        // Implement logic to hide or show the object
        gameObject.SetActive(!isHidden);
    }

    public void DeactiveCoroutine()
    {
        if (_tweenCoroutine != null)
        {
            transform.localScale = _originScale;
            StopCoroutine(_tweenCoroutine);
            _tweenCoroutine = null;
        }
    }

    public void ChangeScale()
    {
        if (this.gameObject.activeInHierarchy)
        {
            // Implement logic to change the scale of the object
            DeactiveCoroutine();
            _targetScale = ConstData.TWEEN_SCALE * transform.localScale;
            _tweenCoroutine = StartCoroutine(ChangeScaleCoroutine());
        }
    }

    private IEnumerator ChangeScaleCoroutine()
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.localScale, _targetScale) < ConstData.OFF_SET);
        _targetScale = _originScale;
        yield return new WaitUntil(() => Vector3.Distance(transform.localScale, _targetScale) < ConstData.OFF_SET);
        DeactiveCoroutine();
    }
}
