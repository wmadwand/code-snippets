using UnityEngine;
using UnityEngine.UI;
using TutorialEventSystem;

public class TwoFingersTap : MonoBehaviour
{
    [SerializeField]
    HoldButton btn01, btn02;
    private bool flag;

    private void Update()
    {
        #region flag
#if UNITY_EDITOR
        flag = btn01.IsHold;
#else
        flag =  btn01.IsHold && btn02.IsHold;
#endif
        #endregion

        if (flag)
        {
            flag = false;

            if (TutorialEventController.Singleton)
            {
                TutorialEventController.Singleton.SpecialEventProceed(SystemMessage.Greetings);
            }
            else
            {
                Debug.Log("TutorialEventSystem.Singleton == null");
            }
        }
    }
}
