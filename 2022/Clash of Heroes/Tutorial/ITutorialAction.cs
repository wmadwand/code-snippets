using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialAction
{
    IEnumerator Play(TutorialStep tutorial, TutorialContext context);
}
