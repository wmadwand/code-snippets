using UnityEngine;
using UnityEngine.EventSystems;

public class BattleCardDropPanelUI : DraggableCardSelector
{
	RectTransform RectTransform;

	void Awake()
    {
		RectTransform = transform.parent.GetComponent<RectTransform>();
	}

    public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);		
		Vector2 point;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.enterEventCamera, out point);
		DragPositionOffset = DraggableObject.transform.localPosition - (Vector3)point;
	}

    public override void OnDrag(PointerEventData data)
	{
		Vector2 point;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, data.position, data.enterEventCamera, out point);
		DraggableObject.transform.localPosition = DragPositionOffset + (Vector3)point;
		Drag(data.position.ScreenPointToFieldPosition());
	}
}