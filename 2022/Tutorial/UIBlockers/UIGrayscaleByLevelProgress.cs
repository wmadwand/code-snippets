using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGrayscaleByLevelProgress : MonoBehaviour
{
    [SerializeField] ComparatorInt QuestLevel;
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
            var effect = Object.GetComponent<UIEffect>();
            if (!effect)
            {
                effect = Object.AddComponent<UIEffect>();
            }
            effect.effectMode = EffectMode.Grayscale;
            effect.effectFactor = 1;
        }
        else
        {
            var effect = Object.GetComponent<UIEffect>();
            if (effect)
            {
                Destroy(effect);
            }
        }
    }
}
