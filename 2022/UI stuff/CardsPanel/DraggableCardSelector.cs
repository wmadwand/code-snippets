using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCardSelector : MonoBehaviour, UnitHighlightColor.IColorPriority
{
    [SerializeField] float SelectionCancelTime;
    [SerializeField] float SelectOffset;
    [SerializeField] float SelectedTimescale;

    public bool ViewIsOverField => ViewOverField.IsOverField;

    protected Card Card;
    protected DeployView ViewOverField;
    protected Vector3 DragablePartInitPosition;
    protected Action DeselectAllCards;
    protected bool Selected;
    protected Vector3 DragPositionOffset;

    protected GameObject DraggableObject;
    protected GameObject SelectedFrame;

    protected BattleCardPanelUI BattleCardPanelUI;
    protected HashSet<Unit> HighlightedUnits = new HashSet<Unit>();

    protected BattleView BattleView => BattleView.Instance;
    protected BattleSimulator BattleSimulator => BattleSimulator.Active;

    int UnitHighlightColor.IColorPriority.Priority => 100000;

    private Vector2 LastDragPoint;

    protected bool Invisible;
    public virtual void SetVisibility(bool visibility)
    {
        Invisible = !visibility;
    }


    public void Init(BattleCardPanelUI battleCardPanelUI, Card card, Action deselectAllCards, DeployView viewOverField)
    {
        Card = card;
        BattleCardPanelUI = battleCardPanelUI;
        if (ViewOverField != null)
        {
            ViewOverField.Invalidate();
            ViewOverField = null;
        }

        DeselectAllCards = deselectAllCards;
        ViewOverField = viewOverField;
    }

    public void Select(bool selected)
    {
        if (DraggableObject == null)
        {
            DraggableObject = BattleCardPanelUI.GetDraggableObject();
            SelectedFrame = BattleCardPanelUI.GetSelectedFrame();
            DragablePartInitPosition = DraggableObject.transform.localPosition;
        }

        Selected = selected;
        DraggableObject.transform.localPosition = DragablePartInitPosition + new Vector3(0, selected ? SelectOffset : 0);
        if (SelectedFrame) { SelectedFrame.SetActive(selected); }

        if (SelectionCancelTime > 0)
        {
            CancelInvoke("CancelSelection");
        }
        if (selected == true)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
            BattleSimulator.State.Input.Enabled = false;
            TimeController.SetTimeScale(SelectedTimescale, this);
            if (SelectionCancelTime > 0)
            {
                Invoke("CancelSelection", SelectionCancelTime);
            }
        }
        else
        {
            if (ViewOverField) { ViewOverField.CancelDeploy(); }
            if (DraggableObject) { DraggableObject.SetActive(true); }
            if (EventSystem.current.currentSelectedGameObject == this)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            TimeController.ResetTimeScale(this);
            BattleSimulator.State.Input.Enabled = true;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        DeselectAllCards?.Invoke();
        Select(true);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        DeselectAllCards?.Invoke();
        Select(true);
        CancelInvoke("CancelSelection");
    }

    public virtual void OnDrag(PointerEventData data) { }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        Select(false);
        ApplyDeploy();
    }

    protected void Drag(Vector2 position)
    {
        LastDragPoint = position;
        ViewOverField.UpdatePoistion(position);
        DraggableObject.SetActive(!ViewOverField.IsOverField);
    }

    public void ForceDeploy(Vector2 position)
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = position;
        OnBeginDrag(eventData);
        OnDrag(eventData);
        Drag(eventData.position.ScreenPointToFieldPosition());
        OnEndDrag(eventData);
    }

    protected virtual void Update()
    {
        if (ViewOverField && ViewOverField.IsOverField)
        {
            UpdateHighlightedUnits(LastDragPoint);
        }
    }

    protected virtual void ApplyDeploy()
    {
        ViewOverField.OnApplyDeploy();
        DraggableObject.SetActive(true);
        ClearHighlighted();
    }

    protected void ClearHighlighted()
    {
        foreach (var u in HighlightedUnits)
        {
            HighlightUnit(u, highlight: false);
        }
        HighlightedUnits.Clear();
    }

    protected void UpdateHighlightedUnits(Vector2 position)
    {
        if (Card.Config.UnitHighlight != null)
        {
            var context = Card.Config.CreateContext(BattleSimulator, Card, position);
            var remainUnits = new List<Unit>(HighlightedUnits);
            foreach (var u in Card.Config.UnitHighlight.Select(context))
            {
                remainUnits.Remove(u);
                if (HighlightedUnits.Add(u))
                {
                    HighlightUnit(u, highlight: true);
                }

            }
            foreach (var u in remainUnits)
            {
                if (HighlightedUnits.Remove(u))
                {
                    HighlightUnit(u, highlight: false);
                }
            }
        }
    }

    protected void HighlightUnit(Unit u, bool highlight)
    {
        var unitView = BattleView?.UnitViews?.GetOrDefault(u);
        if (unitView && unitView.Highlight)
        {
            if (highlight)
            {
                var config = BattleSimulator.State.StatsTable.Battle.UI.UnitHighlighs;
                unitView.Highlight.SetColor(config.Selection, this);
                unitView.Highlight.SetOutlineColor(config.SelectionOutline);
                unitView.Highlight.ShowOutline(true);
            }
            else
            {
                unitView.Highlight.ShowOutline(false);
                unitView.Highlight.RemoveColor(this);
            }
        }
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        var selected = Selected;
        Select(false);
        if (selected)
        {
            var fieldPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y).ScreenPointToFieldPosition();
            ViewOverField.UpdatePoistion(fieldPosition);
            if (ViewOverField.IsOverField)
            {
                ApplyDeploy();
            }
        }
    }

    protected void CancelSelection()
    {
        Select(false);
    }

    public virtual void Invalidate()
    {
        if (ViewOverField)
        {
            ViewOverField.Invalidate();
            ViewOverField = null;
        }
        Destroy(this);
    }
}