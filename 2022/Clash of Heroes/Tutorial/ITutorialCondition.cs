using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialCondition
{
    bool IsMet(TutorialContext context);
}
