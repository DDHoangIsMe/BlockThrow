using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform invisiblePanel;
    public Transform targetObject;
    public Camera mainCamera;

    private bool _isControlTarget = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveObject(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveObject(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(invisiblePanel, eventData.position, null))
        {
            //When mouse of of panel
        }
        else if (!targetObject.GetComponent<StackBlockShooter>().IsDragable())
        {
            //Other action are processing
        }
        else if (_isControlTarget)
        {
            //Trigger shooter action
            targetObject.GetComponent<StackBlockShooter>().ActionEndDrag();
        }
        _isControlTarget = false;
    }

    private void MoveObject(PointerEventData eventData)
    {
        if (!targetObject.GetComponent<StackBlockShooter>().IsDragable())
        {
            return;
        }
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(invisiblePanel, eventData.position, mainCamera, out localPoint))
        {
            _isControlTarget = true;
            // Convert local point to world space and keep the object within the panel
            Vector3 worldPoint = invisiblePanel.transform.TransformPoint(localPoint);
            Rect panelRect = invisiblePanel.rect;
            float rangePanel = (ConstData.SCREEN_WIDTH - ConstData.SHOOTER_SIZE) / 2;
            worldPoint.x = Mathf.Clamp(worldPoint.x, -rangePanel, rangePanel);
            targetObject.position = new Vector3(worldPoint.x, targetObject.position.y, targetObject.position.z);
        }

    }

}
