using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeployView : MonoBehaviour , DeployView.ISetCard, DeployView.IDirectionDeploy
{
    public SpriteRenderer CardIcon;

    void DeployView.ISetCard.Set(Card card)
    {
        CardIcon.sprite = card.Config.Icon;
    }

    Vector2 DeployView.IDirectionDeploy.GetPosition(Vector2 direction)
    {
        return Vector2.zero;
    }
}
