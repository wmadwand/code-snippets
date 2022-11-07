using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Action<ShopItemConfig> SelectItem;
    public ShopItemConfig Config => Item;

    [SerializeField] private ShopItemConfig Item;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI GemsField;
    [SerializeField] private TextMeshProUGUI CostField;

    private Button Button;

    private void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnTap);
        RefreshPrice();
    }



    public void RefreshPrice()
    {
        if (!Item) { return; }
        if (GemsField) GemsField.text = $"+{Item.GetReward(Game.PlayerProfile).Amount}";
        if (CostField) CostField.text = Item.Price.AmountText;

    }

    private void OnTap()
    {
        SelectItem?.Invoke(Item);
    }
}
