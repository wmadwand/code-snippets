using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaTutorialConfig : SerializedScriptableObject
{
    public enum CheckTrigger
    {
        Update,
        QuestFailed,
    }

    public CheckTrigger Trigger;

    [TypesDropdown]
    public ITutorialCondition Condition;
    [TypesDropdown]
    public ITutorialCondition RepeatCondition;

    public List<ITutorialStepConfig> Tutorials;
}
