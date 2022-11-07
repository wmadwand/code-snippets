using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialController
{
    PlayerProfile PlayerProfile { get;  }
    bool IsStepCompleted(string id);
    void Schedule(IEnumerator enumerator, bool inBattle, System.Action onComplete = null);
}
