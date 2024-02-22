using UnityEngine;
using System.Collections.Generic;

namespace TutorialEventSystem
{
    [CreateAssetMenu(fileName = "TutorialPageData", menuName = "TutorialEventSystem/TutorialPage", order = 1)]
    public class TutorialPage : ScriptableObject
    {
        //[SerializeField]
        //string guid;
        //public string GUID { get { return guid; } }

        public PageType type = PageType.Executable;
        public SystemMessage OnEvent = SystemMessage.None;
        public GameButton proceedButton = GameButton.None;
        public Chapter chapter = Chapter.SpiritWorld;
        public PlaceToShow placeToShow = PlaceToShow.Map;

        public bool recoverProgressAfterCrash = true;

        public bool hasTitle;
        public string title = "";
        public int titleFontSize;

        public InfoType infoType;
        public string infoText = "";
        public int infoTextFontSize;
        public Sprite infoImage = null;

        public string callToAction = "";
        public int callToActionFontSize;

        public Sprite spiritImage = null;
        public TooltipPosition tooltipPosition;
        public ButtonType buttonType;

        public bool hasSlavePages;
        public float delayBetweenPages;
        public List<TutorialSlavePage> slavePageList = new List<TutorialSlavePage>();

        public bool IsRecoverableAfterCrash()
        {
            return placeToShow == PlaceToShow.Map && type == PageType.Executable && recoverProgressAfterCrash;
        }
    }

    [System.Serializable]
    public struct TutorialSlavePage
    {
        public TutorialPage page;
    }
}