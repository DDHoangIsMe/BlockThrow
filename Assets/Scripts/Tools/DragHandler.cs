using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public RectTransform invisiblePanel;
    public Transform targetObject;
    public Camera mainCamera;

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveObject(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveObject(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!targetObject.GetComponent<StackBlockShooter>().IsDragable())
        {
            return;
        }
        else
        {
            targetObject.GetComponent<StackBlockShooter>().ActionEndDrag();
        }
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
            // Convert local point to world space and keep the object within the panel
            Vector3 worldPoint = invisiblePanel.transform.TransformPoint(localPoint);
            Rect panelRect = invisiblePanel.rect;
            float rangePanel = (ConstData.SCREEN_WIDTH - ConstData.SHOOTER_SIZE) / 2;
            worldPoint.x = Mathf.Clamp(worldPoint.x, -rangePanel, rangePanel);
            targetObject.position = new Vector3(worldPoint.x, targetObject.position.y, targetObject.position.z);
        }

    }

}
