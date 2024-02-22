using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHideByLevelProgress : MonoBehaviour
{
    [SerializeField] ComparatorInt QuestLevel;
    [SerializeField] bool Show;
    [SerializeField] GameObject Object;

    private void Awake()
    {
        var isMet = QuestLevel.IsMet(Game.PlayerProfile.Progress.LevelIndex);
        if (!Object)
        {
            Object = gameObject;
        }
        if (isMet)
        {
            Object.SetActive(Show);
        }
        else
        {
            Object.SetActive(!Show);
        }
    }
}
