using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialConditions
{
    public class HasChest : ITutorialCondition
    {
        public bool IsMet(TutorialContext context)
        {
            var chests = context.Controller.PlayerProfile.Resources.Chests;
            return chests != null && chests.Any(c => c.Count > 0);
        }
    }

    public class Quest : ITutorialCondition
    {
        [SerializeField] ComparatorInt Condition;

        public bool IsMet(TutorialContext context)
        {
            Debug.LogError("TODO remove quest condition");
            return false;
        }
    }

    public class QuestFailed : ITutorialCondition
    {
        public bool IsMet(TutorialContext context)
        {
            Debug.LogError("TODO remove QuestFailed condition");
            return false;
        }
    }

    public class HasCardForUpgrade : ITutorialCondition
    {
        public bool IsMet(TutorialContext context)
        {
            foreach (var c in context.Controller.PlayerProfile.Cards)
            {
                var upgradeCost = Game.Config.Meta.GetCost(c.Config.Rarity, c.Level);
                if (upgradeCost.PartsRequired <= c.Parts)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class OpenChestCount : ITutorialCondition
    {
        [SerializeField] ComparatorInt Count;

        public bool IsMet(TutorialContext context)
        {
            var chests = context.Controller.PlayerProfile.Resources.Chests;
            return chests != null && Count.IsMet(chests.Sum(c => c.Collected));
        }
    }

    public class Timescale : ITutorialCondition
    {
        [SerializeField] ComparatorFloat Condition;

        public bool IsMet(TutorialContext context)
        {
            return Condition.IsMet(Time.timeScale);
        }
    }

    public class HasTimesScaleChange : ITutorialCondition
    {
        public bool IsMet(TutorialContext context)
        {
            return TimeController.HasScaleChange;
        }
    }

    public class HasAngelBlessing : ITutorialCondition
    {
        public bool IsMet(TutorialContext context)
        {
            return context.Controller.PlayerProfile.Progress.WinStreak > 0;
        }
    }

    public class LevelIndex : ITutorialCondition
    {
        [SerializeField] ComparatorInt Condition;

        public bool IsMet(TutorialContext context)
        {
            return Condition.IsMet(context.Controller.PlayerProfile.Progress.LevelIndex);
        }
    }
}
