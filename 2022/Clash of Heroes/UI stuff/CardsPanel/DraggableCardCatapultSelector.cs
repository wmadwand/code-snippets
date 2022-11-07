using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class DraggableCardCatapultSelector : DraggableCardSelector
{
    [SerializeField] private Material SplineMaterial;
    [SerializeField] private float SplineRangeFactor;
    [SerializeField] private float SplineCurvatureFactor;
    [SerializeField] private float SplineThickness;

    [SerializeField] private float AppearanceTime = 0.2f;
    private readonly int SplineResolution = 100;
    
    protected LineRenderer LineRenderer;
    private Vector3 SplineStartPoint;
    private Camera Camera;
    private Vector3 CamPosition;
    private float AppearancesTime;

    protected virtual void Awake()
    {
        LineRenderer = gameObject.GetComponent<LineRenderer>();
        if (!LineRenderer)
        {
            LineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        LineRenderer.material = SplineMaterial;
        LineRenderer.startWidth = LineRenderer.endWidth = SplineThickness;
        LineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        LineRenderer.receiveShadows = false;
        LineRenderer.textureMode = LineTextureMode.Stretch;
        LineRenderer.sortingOrder = 1000;
        LineRenderer.sortingLayerName = "Effects";
        LineRenderer.positionCount = 0;

        if (Invisible)
        {
            LineRenderer.enabled = false;
        }
        
    }

    public override void SetVisibility(bool visibility)
    {
        base.SetVisibility(visibility);
        if (LineRenderer != null)
        {
            LineRenderer.enabled = visibility;
        }
    }

    private void Start()
    {
        Camera = Camera.main;
        CamPosition = Camera.transform.position;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        SplineStartPoint = eventData.position.ScreenToWorldPosition();
        base.OnBeginDrag(eventData);
        
        Debug.Log($"{eventData.position}");

        if (LineRenderer)
        {
            LineRenderer.material.SetFloat(ShaderConstants.ClipMask, 0.0f);
            LineRenderer.material.DOFloat(1.0f, ShaderConstants.ClipMask, AppearancesTime);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        var touchPoint = eventData.position.ScreenToWorldPosition();
        var splineEndPoint = Vector3.LerpUnclamped(SplineStartPoint, touchPoint, SplineRangeFactor);
        if (ViewOverField.CanActivateOutsideField)
        {
            var p = splineEndPoint.ConvertWorldPositionToField();
            if(!BattleSimulator.State.Field.LevelConfig.IsInsideFieldForSpell(p))
            {
                var startP = SplineStartPoint.ConvertWorldPositionToField();
                var rectInt = BattleSimulator.State.Field.LevelConfig.SizeInTiles;
                var rect = new Rect(rectInt.x, rectInt.y, rectInt.width, rectInt.height);
                Raycast2DUtils.Raycast2DLineRect.LineRectResult result = new Raycast2DUtils.Raycast2DLineRect.LineRectResult();
                Raycast2DUtils.Raycast2DLineRect.RaycastLineRect(startP, p, rect, ref result);
                if (result.Exit != float.PositiveInfinity)
                {
                    p = startP + (p - startP) * result.Exit;
                    splineEndPoint = p.ConvertFieldPositionToWorld();
                }
            }
        }

        var cp1 = Vector3.Lerp(touchPoint, CamPosition, SplineCurvatureFactor);
        var cp2 = Vector3.Lerp(splineEndPoint, CamPosition, SplineCurvatureFactor);

        var points = GeomUtil.Bezier.GetPath(SplineResolution, SplineStartPoint, cp1, cp2, splineEndPoint);
        if (LineRenderer)
        {
            LineRenderer.positionCount = points.Length;
            LineRenderer.SetPositions(points);
        }

        if (Camera)
        {
            DraggableObject.transform.position = Camera.WorldToScreenPoint(splineEndPoint + DragPositionOffset);
        }
        Drag(splineEndPoint.ConvertWorldPositionToField());
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (LineRenderer)
        {
            LineRenderer.positionCount = 0;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        DeselectAllCards?.Invoke();
        Select(true);

        if (LineRenderer)
        {
            LineRenderer.positionCount = 0;
        }
    }
}