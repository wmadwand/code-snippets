using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialActions
{
    public class SetChestTimeTutorialAction : ITutorialAction
    {
        [SerializeField] int Time;
        [SerializeField] Rarity Rarity;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            Game.Services.Tutorial.SetChestTime(Rarity, Time);
            yield break;
        }
    }
}
