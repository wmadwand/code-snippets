using System.Collections.Generic;
using UnityEngine;

namespace TutorialEventSystem
{
    [CreateAssetMenu(fileName = "TutorialPageListData", menuName = "TutorialEventSystem/TutorialPageList", order = 1)]
    public class TutorialPageList : ScriptableObject
    {
        //public float delayBetweenPages;
        public List<TutorialPageLink> pageLinkList = new List<TutorialPageLink>();
    }

    [System.Serializable]
    public struct TutorialPageLink
    {
        public TutorialPage page;
    }
}