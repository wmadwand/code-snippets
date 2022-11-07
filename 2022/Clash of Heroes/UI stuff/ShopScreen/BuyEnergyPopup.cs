using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyEnergyPopup : MonoBehaviour
{
    [SerializeField] ButtonInfo[] Buttons;
    [SerializeField] Button CloseButton;

    [System.Serializable]
    public class ButtonInfo
    {
        public Button Button;
        public TextMeshProUGUI Amount;
        public TextMeshProUGUI Price;
    }

    private SequencePlayer Animator;

    private void Awake()
    {
        var index = 0;
        foreach(var b in Buttons)
        {
            var pack = Game.Config.Meta.EnergyPacks.GetClamp(index);
            b.Price.text = pack.GemsPrice.ToString();
            b.Amount.text = pack.Amount.ToString();
            b.Button.onClick.AddListener(() => {

                Game.Services.Resources.BuyEnergy(pack);
                Close();

            });
            index++;
        }
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
