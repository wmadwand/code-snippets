using UnityEngine;

namespace TutorialEventSystem
{
    public delegate void SystemMessageEventHandler(SystemMessage message);

    public class TutorialEventCollection : MonoBehaviour
    {
        public static event SystemMessageEventHandler SystemMessage;

        public static void OnSystemMessage(SystemMessage message)
        {
            if (SystemMessage != null)
            {
                SystemMessage(message);
            }
        }
    }
}