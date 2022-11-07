using System;
using UnityEngine;
using System.Collections;
using Coffee.UIEffects;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BattleCardPanelUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDeselectHandler
{
    public Card Card { get; private set; }

    [SerializeField] private Canvas Canvas;
    [SerializeField] private Image Icon;
    [SerializeField] private Image Frame;
    [SerializeField] private Image ManaIndicator;
    [SerializeField] private TextMeshProUGUI CardPriceText;
    [SerializeField] UIEffect GrayscaleEffect;

    [Space(10)]
    [SerializeField] private GameObject DraggableObject;
    [SerializeField] private GameObject SelectedFrame;
    [SerializeField] private GameObject View;

    public DraggableCardSelector DropablePart;
    private float ManaPrice;


    public GameObject GetDraggableObject() => DraggableObject;
    public GameObject GetSelectedFrame() => SelectedFrame;
    public GameObject GetView => View;

    private BattleSimulator BattleSimulator => BattleSimulator.Active;

    public void Init(Card card, System.Action deselectAllCards = null, System.Func<Vector2, IExecutorCustomData, bool> onActivateCard = null, DraggableCardSelector selectorPrefab = null)
    {
        Card = card;

        if (Card != null && DraggableObject != null)
        {
            if (selectorPrefab == null)
            {
                if (Card.Config.UseCatapultSelector)
                {
                    selectorPrefab = BattleSimulator.State.VisualConfig.DraggableCardCatapultSelector;
                }
                else
                {
                    selectorPrefab = BattleSimulator.State.VisualConfig.DraggableCardSelector;
                }
            }
            if (DropablePart)
            {
                DropablePart.Invalidate();
            }
            DropablePart = Instantiate(selectorPrefab, transform);

            if (Card.Config.UseCatapultSelector)
            {
                DropablePart.SetVisibility(!Card.Config.InvisibleCatapult);
            }
        }
        if (Card != null)
        {
            Icon.sprite = Card.Config.Icon;
            Frame.sprite = Game.Config.Meta.UI.CardFrameByRariy.GetOrDefault(Card.Config.Rarity);
            CardPriceText.text = Mathf.FloorToInt(Card.GetStat(StatType.ManaPrice)).ToString();

            if (onActivateCard != null && DropablePart != null)
            {
                var overFieldView = Instantiate(Card.Config.DeployView, BattleSimulator.State.Containers.Root);
                overFieldView.Init(Card, onActivateCard);
                DropablePart.Init(this, Card, deselectAllCards, overFieldView);
            }
        }

        Select(false);
        gameObject.SetActive(Card != null);
    }

    public void InitManaIndicator()
    {
        ManaPrice = Card == null ? 0 : 1;
        ManaIndicator.fillAmount = 0;
        GrayscaleEffect.effectFactor = 0;
    }

    public void UpdateManaIndicator(float mana)
    {
        if (ManaPrice <= mana)
        {
            if (GrayscaleEffect.effectFactor > 0)
            {
                GrayscaleEffect.effectFactor = 0;
            }
            if (GrayscaleEffect.colorFactor > 0)
            {
                GrayscaleEffect.colorFactor = 0;
            }
            if (ManaIndicator.fillAmount > 0)
            {
                ManaIndicator.fillAmount = 0;
            }
            if (!Icon.raycastTarget)
            {
                Icon.raycastTarget = true;
            }
            return;
        }
        else
        {
            if (Icon.raycastTarget)
            {
                Icon.raycastTarget = false;
            }
            if (GrayscaleEffect.effectFactor < 1)
            {
                GrayscaleEffect.effectFactor = 1;
                GrayscaleEffect.colorFactor = 0.5f;
            }

            var percent = Math.Abs(mana / ManaPrice - 1);
            ManaIndicator.fillAmount = percent;
        }
    }

    public void Select(bool selected)
    {
        if (DropablePart != null)
        {
            DropablePart.Select(selected);
        }
    }

    public void ForceDeploy()
    {
        if (DropablePart)
        {
            DropablePart.ForceDeploy(Input.mousePosition);
        }
    }

    public BattleCardPanelUI GetViewClone(Card card)
    {
        //TODO: find another safe parent
        var clone = Instantiate(this, transform.parent.parent.parent);

        clone.Card = card;
        clone.Icon.sprite = card.Config.Icon;
        clone.Frame.sprite = Game.Config.Meta.UI.CardFrameByRariy.GetOrDefault(card.Config.Rarity);
        clone.CardPriceText.text = Mathf.FloorToInt(card.GetStat(StatType.ManaPrice)).ToString();
        clone.GetComponent<Canvas>().overrideSorting = false;
        clone.GetComponent<GraphicRaycaster>().enabled = false;

        clone.gameObject.SetActive(true);
        clone.GetDraggableObject().SetActive(true);
        clone.GetView.SetActive(true);

        return clone;
    }

    public Tween MoveTo(Vector3 position, float time, Action callback = null)
    {
        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1, time))
           .Join(transform.DOMove(position, time))
           .SetEase(Ease.Linear)
           .AppendCallback(() => callback())
           ;

        return seq;
    }

    #region Pointer Events Handlers

    public void OnPointerDown(PointerEventData eventData)
    {
        if (DropablePart) { DropablePart.OnPointerDown(eventData); }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (DropablePart) { DropablePart.OnBeginDrag(eventData); }
        if (Canvas) { Canvas.overrideSorting = true; }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (DropablePart) { DropablePart.OnDrag(eventData); }

        var deckPanel = BattlePanelUI.Instance.DeckPanel;
        var isPointerOverDeck = RectTransformUtility.RectangleContainsScreenPoint(deckPanel, eventData.position, BattlePanelUI.Instance.UICam);
        if (isPointerOverDeck && DropablePart && !DropablePart.ViewIsOverField)
        {
            if (DropablePart) { OnPointerDown(eventData); }
            if (Canvas) { Canvas.overrideSorting = false; }
        }
        else
        {
            if (Canvas) { Canvas.overrideSorting = true; }
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (DropablePart) { DropablePart.OnEndDrag(eventData); }
        if (Canvas) { Canvas.overrideSorting = false; }
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        if (DropablePart) { DropablePart.OnDeselect(eventData); }
    }

    #endregion
}