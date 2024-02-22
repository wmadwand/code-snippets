using UnityEngine;
using System.Collections;
using TutorialActions;

public class TutorialHighlightController : MonoBehaviour
{

	[SerializeField] protected GameObject Dark;
	[SerializeField] protected float OriginalHoleSize;
	[SerializeField] protected GameObject CircleHole;
	[SerializeField] protected GameObject SqaureHole;
	[SerializeField] int DarkSortOrderOffser = 11;
	[SerializeField] Transform CenterSpeechPosition;
	[SerializeField] Transform UpCenterSpeechPosition;
	[SerializeField] Transform LeftSpeechPosition;
	[SerializeField] Transform RightSpeechPosition;

	public Transform GetSpeechPosition(Speech.Anchors anchor)
	{
		switch (anchor)
		{
			case Speech.Anchors.Center:
				return CenterSpeechPosition;
			case Speech.Anchors.Left:
				return LeftSpeechPosition;
			case Speech.Anchors.Right:
				return RightSpeechPosition;
			case Speech.Anchors.UpCenter:
				return UpCenterSpeechPosition;
		}
		return CenterSpeechPosition;
	}

	public void SetHighlightedObject(Component obj, bool circleHole)
	{
		var canvas = GetComponent<Canvas>();
		if (canvas)
		{
			canvas.sortingOrder += DarkSortOrderOffser;
		}
		CircleHole.SetActive(circleHole);
		SqaureHole.SetActive(!circleHole);
		Transform origParent = Dark.transform.parent;
		Quaternion origRotation = Dark.transform.rotation;
		Dark.transform.SetParent(obj.transform, false);
		Canvas.ForceUpdateCanvases();

		Dark.transform.SetParent(origParent, true);
		Dark.transform.rotation = origRotation;

		float width = OriginalHoleSize;
		float height = OriginalHoleSize;
		var rects = obj.gameObject.GetComponentsInChildren<RectTransform>();
		foreach (var rect in rects)
		{
			if (rect.rect.width > width)
			{
				width = rect.rect.width;
			}
			if (rect.rect.height > height)
			{
				height = rect.rect.height;
			}
		}
		float xScale = Mathf.Max(width / OriginalHoleSize, 0.5f);
		float yScale = Mathf.Max(height / OriginalHoleSize, 0.5f);

		Dark.transform.SetScaleX(xScale);
		Dark.transform.SetScaleY(yScale);
		Dark.transform.SetAsFirstSibling();
		Dark.SetActive(true);
	}

	public void Move(Vector2 offset)
	{
		Dark.transform.localPosition += (Vector3)offset;
	}

	public void SetSize(Vector2 size)
	{
		float xScale = Mathf.Max(size.x / OriginalHoleSize, 0.5f);
		float yScale = Mathf.Max(size.y / OriginalHoleSize, 0.5f);

		Dark.transform.SetScaleX(xScale);
		Dark.transform.SetScaleY(yScale);
	}

}
