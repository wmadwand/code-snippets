using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDaily : MonoBehaviour
{        
    [SerializeField] private TextMeshProUGUI GoldAmount;
    [SerializeField] private ChestRewardItem CardPanel;
    [SerializeField] private GameObject GoldPanel;
    [SerializeField] private GameObject[] ShowWhenCollected;
    [SerializeField] private GameObject[] HideWhenCollected;
    [SerializeField] private TextMeshProUGUI TimerToNextReward;

    private ShopItemDailyConfig Item => Game.Config.Meta.DailyReward;
    private Button Button;

    private void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnTap);
        Refresh();
        Game.Services.OnTrasactionCompleted += Refresh;
    }

    private void OnDestroy()
    {
        Game.Services.OnTrasactionCompleted -= Refresh;
    }

    public void Refresh()
    {
        if (!Item) { gameObject.SetActive(false); return; }
        Game.Services.Resources.UpdateDailyReward();
        foreach(var go in ShowWhenCollected)
        {
            go.SetActive(Game.PlayerProfile.Resources.DailyReward.Collected);
        }
        foreach (var go in HideWhenCollected)
        {
            go.SetActive(!Game.PlayerProfile.Resources.DailyReward.Collected);
        }
        var reward = Item.GetReward(Game.PlayerProfile);
        var gold = (reward is GoldReward);
        GoldPanel.SetActive(gold);
        CardPanel.gameObject.SetActive(!gold);
        CardPanel.Init(reward);
        GoldAmount.text = $"+{reward.Amount}";

        CancelInvoke(nameof(UpdateTimer));
        if (Game.PlayerProfile.Resources.DailyReward.Collected)
        {
            InvokeRepeating(nameof(UpdateTimer), 0, 1);
        }
    }

    void UpdateTimer()
    {
        TimerToNextReward.text = Localization.Get("FreeBonusTimer_Text",  Game.PlayerProfile.Resources.DailyReward.RemainTime.ConvertToRemainTimeString());
        Game.Services.Resources.UpdateDailyReward();
    }

    private void OnTap()
    {
        Game.Services.Resources.CollectDailyReward(Item);
    }
}
