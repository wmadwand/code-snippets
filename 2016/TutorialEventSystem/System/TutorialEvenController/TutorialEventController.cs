/// <summary>
/// Programmed by WMADWAND
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TutorialEventSystem
{
    [RequireComponent(typeof(TutorialCrashRecovery))]
    [RequireComponent(typeof(TutorialGameButtons))]
    public class TutorialEventController : MonoBehaviour
    {
        #region Properties
        [SerializeField]
        NetworkController _networkController;
        public NetworkController networkController { get { return _networkController; } }

        [HideInInspector]
        public TutorialCrashRecovery tutorialCrashRecovery { get; private set; }

        [SerializeField]
        TutorialPageList pageListData;
        public TutorialPageList PageListData { get { return pageListData; } }

        [Header("Tutorial page panel")]
        [SerializeField]
        GameObject mainPanel;

        [SerializeField]
        RectTransform tooltipPanelRect;
        [SerializeField]
        RectTransform tooltipGuideRect;

        [SerializeField]
        Text titleText;

        [SerializeField]
        Text infoText;
        [SerializeField]
        Text infoTextHigh;
        [SerializeField]
        Image infoImage;

        [SerializeField]
        Text callToActionText;
        [SerializeField]
        Image spiritImage;

        [SerializeField]
        Button fingerButton;
        [SerializeField]
        Button coverButton;
        [SerializeField]
        Image coverImage;

        [Header("Special stuff")]
        [SerializeField]
        GameObject twoFingersTapPanel;
        [SerializeField]
        GameObject tapToJumpPanel;

        private Button chosenButton;

        private SystemMessageEvent systemMessageEvent;

        [HideInInspector]
        public bool gotEventInProcess { get; private set; }
        #endregion

        #region Singleton
        private static TutorialEventController _singleton;
        public static TutorialEventController Singleton
        {
            get
            {
                if (!_singleton)
                {
                    _singleton = FindObjectOfType<TutorialEventController>();
                }

                return _singleton;
            }
        }
        #endregion

        #region Base methods
        private void Awake()
        {
            _singleton = this;
            tutorialCrashRecovery = GetComponent<TutorialCrashRecovery>();
            chosenButton = fingerButton;
        }

        private void Start()
        {
            //DontDestroyOnLoad(this);
            AddEventsListeners();

            mainPanel.SetActive(false);
            twoFingersTapPanel.SetActive(false);
            tapToJumpPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            RemoveEventsListeners();
        }
        #endregion

        #region Events methods
        private void AddEventsListeners()
        {
            systemMessageEvent = new SystemMessageEvent(networkController);
            TutorialEventCollection.SystemMessage += systemMessageEvent.OnEvent;
        }

        private void RemoveEventsListeners()
        {
            if (systemMessageEvent != null)
            {
                TutorialEventCollection.SystemMessage -= systemMessageEvent.OnEvent;
            }
        }

        public void SpecialEventProceed(SystemMessage _message)
        {
            if (systemMessageEvent != null && gotEventInProcess)
            {
                systemMessageEvent.EventProceed(_message);
            }
        }

        public void TerminateEventInProcess()
        {
            gotEventInProcess = false;
        }
        #endregion

        #region Launchers
        public void RunEvent(SystemMessage _message)
        {
            TutorialEventCollection.OnSystemMessage(_message);
        }

        public void TestLaunchOnSystemMessage(TutorialPage _page)
        {
            TutorialEventCollection.OnSystemMessage(_page.OnEvent);
        }

        public void TestLaunchOnSystemMessage02()
        {
            TutorialEventCollection.OnSystemMessage(SystemMessage.OpenSpellChest);
        }
        #endregion

        #region Service methods
        public void SetupPagePanel(TutorialPage _tutorialPage)
        {
            bool _hasTitle = _tutorialPage.hasTitle;
            if (_hasTitle)
            {
                titleText.text = _tutorialPage.title;
                titleText.fontSize = _tutorialPage.titleFontSize;
            }

            titleText.gameObject.SetActive(_hasTitle);

            switch (_tutorialPage.infoType)
            {
                case InfoType.Text:
                    {
                        infoImage.gameObject.SetActive(false);
                        infoTextHigh.gameObject.SetActive(!_hasTitle);
                        infoText.gameObject.SetActive(_hasTitle);

                        Text currInfoText = _tutorialPage.hasTitle ? infoText : infoTextHigh;
                        currInfoText.text = _tutorialPage.infoText;

                        currInfoText.fontSize = _tutorialPage.infoTextFontSize;
                    }
                    break;
                case InfoType.Image:
                    {
                        infoImage.gameObject.SetActive(true);
                        infoTextHigh.gameObject.SetActive(false);
                        infoText.gameObject.SetActive(false);
                    }
                    break;
            }

            callToActionText.text = _tutorialPage.callToAction;
            callToActionText.fontSize = _tutorialPage.callToActionFontSize;

            spiritImage.sprite = _tutorialPage.spiritImage;

            switch (_tutorialPage.tooltipPosition)
            {
                case TooltipPosition.RightBottom:
                    {
                        tooltipPanelRect.anchorMin = new Vector2(1, 0);
                        tooltipPanelRect.anchorMax = new Vector2(1, 0);
                        tooltipPanelRect.pivot = new Vector2(1f, 0f);
                        tooltipPanelRect.anchoredPosition = new Vector2(0, 0);

                        tooltipGuideRect.anchorMin = new Vector2(1f, 0);
                        tooltipGuideRect.anchorMax = new Vector2(1f, 0);
                        tooltipGuideRect.anchoredPosition = new Vector2(-188, 125);
                        break;
                    }
                case TooltipPosition.LeftBottom:
                    {
                        tooltipPanelRect.anchorMin = new Vector2(0, 0);
                        tooltipPanelRect.anchorMax = new Vector2(0, 0);
                        tooltipPanelRect.pivot = new Vector2(1f, 0f);
                        tooltipPanelRect.anchoredPosition = new Vector2(tooltipPanelRect.rect.width, 0);

                        tooltipGuideRect.anchorMin = new Vector2(1f, 0);
                        tooltipGuideRect.anchorMax = new Vector2(1f, 0);
                        tooltipGuideRect.anchoredPosition = new Vector2(244, 125);
                        break;
                    }

                case TooltipPosition.CenterBottom:
                    {
                        tooltipPanelRect.anchorMin = new Vector2(0.5f, 0);
                        tooltipPanelRect.anchorMax = new Vector2(0.5f, 0);
                        tooltipPanelRect.pivot = new Vector2(1f, 0f);
                        tooltipPanelRect.anchoredPosition = new Vector2(tooltipPanelRect.rect.width / 2, 0);

                        tooltipGuideRect.anchorMin = new Vector2(0.5f, 0);
                        tooltipGuideRect.anchorMax = new Vector2(0.5f, 0);
                        tooltipGuideRect.anchoredPosition = new Vector2(tooltipGuideRect.rect.width / 2, 125);
                        break;
                    }
            }

            switch (_tutorialPage.buttonType)
            {
                case ButtonType.Finger:
                    {
                        chosenButton = fingerButton;
                        chosenButton.gameObject.SetActive(true);
                        break;
                    }
                case ButtonType.Cover:
                    {
                        chosenButton = coverButton;
                        fingerButton.gameObject.SetActive(false);
                        break;
                    }
            }

            if (_tutorialPage.OnEvent == SystemMessage.Greetings)
            {
                twoFingersTapPanel.SetActive(true);
            }
            else
            {
                twoFingersTapPanel.SetActive(false);
            }

            if (_tutorialPage.OnEvent == SystemMessage.NowJump)
            {
                tapToJumpPanel.SetActive(true);
            }
            else
            {
                tapToJumpPanel.SetActive(false);
            }
        }

        public TutorialPage GetPageData(SystemMessage _message)
        {
            TutorialPageLink _data = PageListData.pageLinkList.Find(m => (m.page != null && m.page.OnEvent == _message));
            return _data.page != null ? _data.page : null;
        }

        public void LaunchCurrentPage(TutorialPage _page, SystemMessage _message, GameObject proceedButton, SystemMessageEventHandler eventProceed)
        {
            SetupPagePanel(_page);
            chosenButton.AddButtonListener(eventProceed, _message);
            GamePause(_page, proceedButton);
        }

        public void GamePause(TutorialPage tPage, GameObject proceedButton)
        {
            bool specialPage = false;

            if (Application.isPlaying) // available only in Play mode
            {
                if (proceedButton == null)
                {
                    Debug.LogError("ProceedButton == null");
                    gotEventInProcess = false;

                    return;
                }

                Time.timeScale = 0;

                specialPage = tPage.OnEvent == SystemMessage.Greetings || tPage.OnEvent == SystemMessage.NowJump;
            }

            coverImage.enabled = true;
            tooltipPanelRect.gameObject.SetActive(true);
            mainPanel.SetActive(true);

            if (tPage.buttonType == ButtonType.Finger)
            {
                Transform _tr = proceedButton.transform.Find("#FingerBtnPos");
                chosenButton.gameObject.transform.position = _tr != null ? _tr.position : proceedButton.transform.position;
                fingerButton.gameObject.SetActive(true);
            }
            else
            {
                fingerButton.gameObject.SetActive(false);
            }

            if (Application.isPlaying) // available only in Play mode
            {
                if (specialPage)
                {
                    chosenButton.onClick.RemoveAllListeners();
                }

                gotEventInProcess = true;
            }
        }

        public void GameResume(GameObject _button, TutorialPage _tPage, bool singlePage)
        {
            if (singlePage)
            {
                mainPanel.SetActive(false);
                tooltipPanelRect.gameObject.SetActive(true);
                if (_tPage.buttonType == ButtonType.Finger)
                {
                    fingerButton.gameObject.SetActive(true);
                }
                else
                {
                    fingerButton.gameObject.SetActive(false);
                }
            }
            else
            {
                tooltipPanelRect.gameObject.SetActive(false);
                coverImage.enabled = false;
                fingerButton.gameObject.SetActive(false);
            }

            if (Application.isPlaying) // available only in Play mode
            {
                if (_button != null)
                {
                    TutorialGameButtons.Singleton.ClickButton(_tPage.proceedButton, _button);
                }
                else
                {
                    Debug.LogError("Proceed button is null and has not been clicked!");
                }

                chosenButton.onClick.RemoveAllListeners();

                gotEventInProcess = false;
                Time.timeScale = 1;
            }
        }
        #endregion
    }
}