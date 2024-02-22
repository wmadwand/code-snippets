using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TutorialEventSystem
{
    public static class TutorialExtensions
    {
        public const string tPageIdToRecoverAfterCrash = "TutorialPageIdToRecoverAfterCrash";

        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static void AddButtonListener(this Button btn, SystemMessageEventHandler _action, SystemMessage _message)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { _action(_message); });
        }

        //public static void SetGUID(this SerializedProperty _prop)
        //{
        //    System.Guid guid = System.Guid.NewGuid();
        //    _prop.stringValue = guid.ToString();
        //}
    }
}