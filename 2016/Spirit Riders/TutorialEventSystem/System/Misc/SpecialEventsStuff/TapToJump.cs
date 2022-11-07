using UnityEngine;
using TutorialEventSystem;

public class TapToJump : MonoBehaviour
{
    public void ActivateJump()
    {
        if (TutorialGameButtons.Singleton && TutorialEventController.Singleton)
        {
            TutorialGameButtons.Singleton.ClickButton(GameButton.Jump);
            TutorialEventController.Singleton.SpecialEventProceed(SystemMessage.NowJump);
        }
        else
        {
            Debug.LogError("TutorialGameButtons.Singleton or/and TutorialEventController.Singleton == null");
        }
    }
}
