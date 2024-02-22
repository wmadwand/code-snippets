using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTutorialConfig : SerializedScriptableObject
{
    [TypesDropdown]
    public List<ITutorialStepConfig> InBattleTutorials;
    [TypesDropdown]
    public List<ITutorialStepConfig> OutOfBattleTutorials;

    public LevelTutorialConfig[] AdditionalTutorials;
}
