using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MainMenuScreen, ILocalizationContent
{
    public enum InAppTypes
    {
        Consumable,
        Subscription
    }

    [SerializeField] private ShopItem[] ShopItems;
    [SerializeField] private Button CloseButton;
    [SerializeField] private GameObject[] HideInGemsMode;
    [SerializeField] private GameObject ResourcePanel;
    [SerializeField] private VerticalLayoutGroup ContentVerticalLayoutGroup;
    [SerializeField] private RectOffset GemsContentVerticalLayoutGroup;
    [SerializeField] private ShopBuyConfirmation BuyConfirmation;
    [SerializeField] private float ScrollToItemSpeed = .5f;
    private Sequence ScrollToItemSequence;

    private void Awake()
    {
        CloseButton.onClick.AddListener(OnClose);

        Localization.OnLanguageChange += RefreshLocalizationContent;
    }

    private void OnDestroy()
    {
        Localization.OnLanguageChange -= RefreshLocalizationContent;
    }

    public override void Init()
    {
        foreach (var item in ShopItems)
        {
            item.SelectItem = OnSelectItem;
        }
    }

    public void InitContextModeGems()
    {
        Init();
        CloseButton.gameObject.SetActive(true);
        foreach (var go in HideInGemsMode)
        {
            go.SetActive(false);
        }
        Instantiate(ResourcePanel, transform);
        ContentVerticalLayoutGroup.padding = GemsContentVerticalLayoutGroup;
    }

    private void OnSelectItem(ShopItemConfig shopItem)
    {
        var shopItemReward = shopItem.GetReward(Game.PlayerProfile);
        if (BuyConfirmation && BuyConfirmation.IsRequiredFor(shopItemReward))
        {
            MainMenuUIController.ShowPopup(Game.Config.Meta.UI.ConfirmBuyPopup)?.Init(shopItem);
            return;
        }

        Game.Services.Resources.BuyShopItem(shopItem);
    }

    private void OnClose()
    {
        Destroy(gameObject);
    }

    public void RefreshLocalizationContent()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }   

    public void ScrollToItem<T>() where T : IReward
    {
        var target = ShopItems.FirstOrDefault(i => i.gameObject.activeSelf && i.Config.GetReward(Game.PlayerProfile).GetType() == typeof(T));

        if (!target) { return; }

        if (ScrollToItemSequence != null && ScrollToItemSequence.IsPlaying())
        {
            ScrollToItemSequence.Kill();
        }

        var targetRect = target.transform.parent.parent.GetComponent<RectTransform>();
        var scrollRect = GetComponentInChildren<ScrollRect>();
        var content = scrollRect.content;

        Canvas.ForceUpdateCanvases();

        var contentPos = scrollRect.transform.InverseTransformPoint(content.position);
        var targetPos = scrollRect.transform.InverseTransformPoint(targetRect.position);
        var resultPos = contentPos - targetPos;
        var contentMaxY = content.sizeDelta.y;
        var finalPos = resultPos.y > contentMaxY ? contentMaxY : resultPos.y;

        ScrollToItemSequence = DOTween.Sequence();
        ScrollToItemSequence.PrependCallback(() => scrollRect.movementType = ScrollRect.MovementType.Clamped)
           .Append(content.DOLocalMoveY(finalPos, ScrollToItemSpeed))
           .AppendCallback(() => Canvas.ForceUpdateCanvases())
           .AppendInterval(.01f)
           .AppendCallback(() => scrollRect.movementType = ScrollRect.MovementType.Elastic)
           ;
    }
}