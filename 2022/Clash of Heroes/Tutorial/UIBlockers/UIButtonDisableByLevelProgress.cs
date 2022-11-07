using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonDisableByLevelProgress : MonoBehaviour
{
    [SerializeField] ComparatorInt QuestLevel;
    [SerializeField] bool Interactable;
    [SerializeField] Button Button;

    private void Awake()
    {
        var isMet = QuestLevel.IsMet(Game.PlayerProfile.Progress.LevelIndex);
        if (!Button)
        {
            Button = gameObject.GetComponent<Button>();
        }
        if (isMet)
        {
            Button.interactable = Interactable;
        }
        else
        {
            Button.interactable = !Interactable;
        }
    }
}
