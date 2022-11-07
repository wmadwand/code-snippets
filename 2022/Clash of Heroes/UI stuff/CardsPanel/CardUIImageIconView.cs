using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUIImageIconView : MonoBehaviour , DeployView.ISetCard
{
    public Image CardIcon;

    void DeployView.ISetCard.Set(Card card)
    {
        CardIcon.sprite = card.Config.Icon;
    }

}
