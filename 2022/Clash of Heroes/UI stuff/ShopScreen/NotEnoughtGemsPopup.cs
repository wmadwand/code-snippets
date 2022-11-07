using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughtGemsPopup : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI GemsAmount;
    [SerializeField] Button GoToShop;
    [SerializeField] Button CloseButton;

    private SequencePlayer Animator;

    public void Init(int gems, Transform contextShopParent = null)
    {
        if (GemsAmount) { GemsAmount.text = gems.ToString(); }

        GoToShop.onClick.RemoveAllListeners();
        GoToShop.onClick.AddListener(() =>
        {
            MainMenuUIController.OpenShop<GemsReward>(contextShopParent);
            Close();
        });

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(Close);
        
        Animator = GetComponent<SequencePlayer>();
    }

    void Close()
    {
        if (Animator != null && Animator.PlayAnimation("Close"))
        {
            return;
        }
        
        Destroy(gameObject);
    }
}
