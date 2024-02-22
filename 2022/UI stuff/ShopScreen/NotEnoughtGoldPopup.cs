using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughtGoldPopup : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI GoldAmount;
    [SerializeField] TMPro.TextMeshProUGUI GemsAmount;
    [SerializeField] Button BuyButton;
    [SerializeField] Button CloseButton;

    private SequencePlayer Animator;

    private void ClosePopup()
    {
        if (!(Animator != null && Animator.PlayAnimation("Close")))
        {
            Destroy(gameObject);
        }
    }

    public void Init(int gold, System.Action<bool> onCompleted)
    {
        Animator = GetComponent<SequencePlayer>();
        
        GemsAmount.text = Game.Config.Meta.GetGoldPriceInGems(gold).ToString();
        GoldAmount.text = Localization.Get("NotEnougthGoldPopup_Text", gold);

        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(() =>
        {
            if (Game.Services.Resources.BuyGold(gold))
            {
                onCompleted?.Invoke(true);
            }
            else
            {
                onCompleted?.Invoke(false);
            }

            ClosePopup();
        });

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            ClosePopup();

            onCompleted?.Invoke(false);
        });
    }
}