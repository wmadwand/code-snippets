using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialActions
{

    public class SetTimeScale : ITutorialAction
    {
        public float TimeScale;
        public bool ResetCurrentScaleChanger;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            Time.timeScale = TimeScale;
            if (ResetCurrentScaleChanger)
            {
                TimeController.ResetTimeScale();
            }
            yield break;
        }
    }

}
