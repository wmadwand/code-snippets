using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialStepConfig
{
    string Id { get; }
    ITutorialCondition Condition { get; }
    ITutorialAction Action { get;  }
}


public class TutorialStepConfig : ITutorialStepConfig
{
    public TutorialStepConfig() { }
    public TutorialStepConfig(ITutorialCondition condition, ITutorialAction action)
    {
        Condition = condition;
        Action = action;
    }

    [SerializeField] string Id = System.Guid.NewGuid().ToString();
    [SerializeField, FoldoutGroup("Config", Expanded = false)] ITutorialCondition Condition;
    [SerializeField, FoldoutGroup("Config", Expanded = false)] ITutorialAction Action;

    string ITutorialStepConfig.Id => Id;
    ITutorialCondition ITutorialStepConfig.Condition => Condition;
    ITutorialAction ITutorialStepConfig.Action => Action;
}

public class TutorialStepConfigScriptableObject : SerializedScriptableObject, ITutorialStepConfig
{
    [SerializeField] ITutorialCondition Condition;
    [SerializeField] ITutorialAction Action;

    string ITutorialStepConfig.Id => name;
    ITutorialCondition ITutorialStepConfig.Condition => Condition;
    ITutorialAction ITutorialStepConfig.Action => Action;
}
