using UnityEngine;
using TutorialEventSystem;

public class TutorialLevelActivator : MonoBehaviour
{
    public SystemMessage tutorialEvent;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (TutorialEventController.Singleton)
            {
                TutorialEventController.Singleton.RunEvent(tutorialEvent);
                Destroy(this);
            }
            else
            {
                Debug.Log("TutorialEventSystem.Singleton == null");
            }
        }
    }
}