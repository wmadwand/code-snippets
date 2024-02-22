using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace TutorialEventSystem
{
    public class SystemMessageEvent
    {
        #region Properties
        private NetworkController networkController;
        private GameObject proceedButton;

        private TutorialPage currTutorialPage;
        private TutorialPage currTutorialPageExe;

        private int currPageNum;
        private List<TutorialSlavePage> currSlavePageList = new List<TutorialSlavePage>();

        private float delayBetweenPages;
        #endregion

        #region Base methods
        public SystemMessageEvent(NetworkController _networkController)
        {
            networkController = _networkController;
        }

        public void OnEvent(SystemMessage _message)
        {
            if (IsNotAllowedMessage(_message))
            {
                return;
            }

            networkController.StartCoroutine(OnEventHandler(_message));
        }

        IEnumerator OnEventHandler(SystemMessage _message)
        {
            int counter = 0;

            while (TutorialEventController.Singleton.gotEventInProcess)
            {
                counter++;

                if (counter > 5000)
                {
                    Debug.LogError("The event in process has stopped responding and has been terminated.");
                    Debug.LogError("The next event has been run then.");
                    TutorialEventController.Singleton.TerminateEventInProcess();
                }

                yield return null;
            }

            if (currPageNum == 0)
            {
                TutorialPage _tPage = TutorialEventController.Singleton.GetPageData(_message);
                if (_tPage == null)
                {
                    Debug.LogErrorFormat("For TutorialEvent = {0} we have no any Tutorial Pages! Check your TutorialPageListData as well.", _message);
                    yield break;
                }
                currTutorialPage = currTutorialPageExe = _tPage;
            }

            switch (_message)
            {
                case SystemMessage.None: Debug.LogErrorFormat("For executable TutorialPage = {0} OnEvent = SystemMessage.None is not allowed! Check the page.", currTutorialPage.title); break;

                // place to show = Level

                case SystemMessage.Greetings: GreetingsHandler(_message); break;
                case SystemMessage.NowJump: NowJumpHandler(_message); break;

                case SystemMessage.FirstStep: FirstStepHandler(_message); break;
                case SystemMessage.NowGetWeapon: NowGetWeaponHandler(_message); break;
                case SystemMessage.NowUseWeapon: NowUseWeaponHandler(_message); break;
                case SystemMessage.NowBecomeSpirit: NowBecomeSpiritHandler(_message); break;
                case SystemMessage.NowUseAbility: NowUseAbilityHandler(_message); break;
                case SystemMessage.YouAreDone: YouAreDoneHandler(_message); break;

                case SystemMessage.NowAutoAttack: NowAutoAttackHandler(_message); break;
                case SystemMessage.RoundFinish: RoundFinishHandler(_message); break;

                case SystemMessage.HasNotUsedSpirit: HasNotUsedSpiritHandler(_message); break;
                case SystemMessage.ZeroDamageWeapon: ZeroDamageWeaponHandler(_message); break;

                // place to show = Map

                case SystemMessage.RevealMap: RevealMapHandler(_message); break;
                case SystemMessage.NotEnoughPoints: NotEnoughPointsHandler(_message); break;
                case SystemMessage.GotEnoughPoints: GotEnoughPointsHandler(_message); break;
                case SystemMessage.ImproveAirSpirit: ImproveAirSpiritHandler(_message); break;

                case SystemMessage.RevealMapCell0102: RevealMapCell0102Handler(_message); break;
                //case SystemMessage.RevealMapCell0103: RevealMapCell0103Handler(_message); break; // unnecessary
                case SystemMessage.RevealMapCell0104: RevealMapCell0104Handler(_message); break;    // -- GOOD; TEMPORARILY DISABLED
                case SystemMessage.RevealMapCell0105: RevealMapCell0105Handler(_message); break; // -- GOOD; TEMPORARILY DISABLED
                case SystemMessage.RevealMapCell0115: RevealMapCell0115Handler(_message); break;

                case SystemMessage.OpenSpellChest: OpenSpellChestHandler(_message); break; // -- GOOD; TEMPORARILY DISABLED
                //case SystemMessage.AddEarthSpirit: CommonSinglePageHandler(_message); break; // -- USELESS; REMOVE IT
                case SystemMessage.AnomalySpellCollect: AnomalySpellCollectHandler(_message); break;

                case SystemMessage.AnomalyClose: AnomalyClosetHandler(_message); break;
                case SystemMessage.AnomalyRecover: AnomalyRecoverHandler(_message); break;

                default: Debug.LogErrorFormat("For TutorialEvent = {0} no any handlers have been found! Check the code in the script.", _message); break;
            }
        }

        public void EventProceed(SystemMessage _message)
        {
            if (currSlavePageList.Count > 0 && currPageNum + 1 < currSlavePageList.Count)
            {
                TutorialEventController.Singleton.GameResume(proceedButton, currTutorialPage, false);
                networkController.StartCoroutine(LaunchNextPageFromSeries(_message)); // TODO: Is this the right scheme/approach ???
            }
            else
            {
                TutorialEventController.Singleton.GameResume(proceedButton, currTutorialPage, true);
                currPageNum = 0;
                delayBetweenPages = 0;

                if (currSlavePageList.Count > 0) // series of pages
                {
                    TutorialPage mainPageInSeries = currSlavePageList[0].page;

                    if (mainPageInSeries.IsRecoverableAfterCrash())
                    {
                        //mainPageInSeries.showingFinished = true;

                        PlayerPrefs.DeleteKey(TutorialExtensions.tPageIdToRecoverAfterCrash);
                    }
                    currSlavePageList.Clear();
                }
                else // single page
                {
                    if (currTutorialPage.IsRecoverableAfterCrash())
                    {
                        PlayerPrefs.DeleteKey(TutorialExtensions.tPageIdToRecoverAfterCrash);
                        //currTutorialPage.showingFinished = true;
                    }
                }
            }
        }
        #endregion

        #region Event handlers
        void CommonSinglePageHandler(SystemMessage _message)
        {
            proceedButton = TutorialGameButtons.Singleton.FindButton(currTutorialPage.proceedButton);

            if (proceedButton != null)
            {
                TutorialEventController.Singleton.LaunchCurrentPage(currTutorialPage, _message, proceedButton, EventProceed);
            }
            else
            {
                Debug.LogErrorFormat("The page for TutorialEvent = {0} is not available because of: Proceed button not found on the Scene!", _message);
            }

            if (currTutorialPage.IsRecoverableAfterCrash() /*&& !currTutorialPage.showingStarted*/)
            {
                PlayerPrefs.SetInt(TutorialExtensions.tPageIdToRecoverAfterCrash, currTutorialPage.GetInstanceID());

                //currTutorialPage.showingStarted = true;
            }
        }

        void CommonSeriesOfPagesHandler(SystemMessage _message)
        {
            if (currTutorialPageExe.hasSlavePages && currTutorialPageExe.slavePageList.Count > 0)
            {
                if (currPageNum == 0)
                {
                    currSlavePageList = new List<TutorialSlavePage>(currTutorialPageExe.slavePageList);
                    currSlavePageList.Insert(0, new TutorialSlavePage { page = currTutorialPageExe });
                    delayBetweenPages = currTutorialPageExe.delayBetweenPages;
                }

                currTutorialPage = currSlavePageList[currPageNum].page;
                if (currTutorialPage != null)
                {
                    CommonSinglePageHandler(_message);
                }
                else
                {
                    Debug.LogErrorFormat("For TutorialEvent = {0} the page with index = {1} is Null and has been skipped!", _message, currPageNum);
                    currPageNum++;
                    OnEvent(_message);
                }
            }
            else
            {
                Debug.LogErrorFormat("For TutorialEvent = {0} we have no any Slave pages but checkbox SlavePages is marked! The whole series has been skipped.\nUncheck SlavePages or add Slave pages items.", _message, currPageNum);
            }
        }

        void GreetingsHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }

        void NowJumpHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }

        void FirstStepHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }

        void NowGetWeaponHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }

        void NowUseWeaponHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSeriesOfPagesHandler(_message); });
        }

        void NowBecomeSpiritHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSeriesOfPagesHandler(_message); });
        }
        void NowUseAbilityHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }
        void YouAreDoneHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }


        void PageWithCheckLevelLoading(SystemMessage _message, Action callback)
        {
            if (LoadingScreenController.Singleton)
            {
                networkController.StartCoroutine(CheckLevelLoading(_message, callback));
            }
        }

        void PageWithCheckMapLoading(SystemMessage _message, Action callback)
        {
            if (LoadingScreenController.Singleton)
            {
                networkController.StartCoroutine(CheckMapLoading(_message, callback));
            }
        }

        void NowAutoAttackHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }


        void RoundFinishHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSeriesOfPagesHandler(_message); });
        }

        void HasNotUsedSpiritHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }


        void ZeroDamageWeaponHandler(SystemMessage _message)
        {
            PageWithCheckLevelLoading(_message, () => { CommonSinglePageHandler(_message); });
        }

        void RevealMapHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }

        void RevealMapCell0102Handler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }

        void RevealMapCell0103Handler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSeriesOfPagesHandler(_message); });
            }
        }

        void RevealMapCell0104Handler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }


        void RevealMapCell0105Handler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSeriesOfPagesHandler(_message); });
            }
        }

        void RevealMapCell0115Handler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }

        void OpenSpellChestHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSeriesOfPagesHandler(_message); });
            }
        }

        void AnomalySpellCollectHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }

        void AnomalyClosetHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }

        void AnomalyRecoverHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }


        void NotEnoughPointsHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }

        void GotEnoughPointsHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSinglePageHandler(_message); });
            }
        }

        void ImproveAirSpiritHandler(SystemMessage _message)
        {
            if (!IsNotAllowedMessage(_message))
            {
                PageWithCheckMapLoading(_message, () => { CommonSeriesOfPagesHandler(_message); });
            }
        }
        #endregion

        #region Service methods
        IEnumerator LaunchNextPageFromSeries(SystemMessage _message)
        {
            yield return new WaitForSeconds(delayBetweenPages);

            currPageNum++;
            OnEvent(_message);
        }

        IEnumerator CheckLevelLoading(SystemMessage _message, Action callback)
        {
            while (true)
            {
                if (LoadingScreenController.Singleton.IsLevelLoaded)
                {
                    callback();
                    break;
                }

                yield return null;
            }
        }

        IEnumerator CheckMapLoading(SystemMessage _message, Action callback)
        {
            while (true)
            {
                if (LoadingScreenController.Singleton.IsCurrentMapLoaded())
                {
                    yield return null;

                    callback();
                    break;
                }

                yield return null;
            }
        }

        bool IsNotAllowedMessage(SystemMessage _message)
        {
            bool _m1 = false;
            bool _m2 = false;

            try // TODO: replace it with the better solution
            {
                switch (_message)
                {
                    case SystemMessage.RevealMap:
                        {
                            _m1 = networkController.settingsGame.m_Map[1].status == 1;
                            _m2 = networkController.settingsGame.m_Map[2].status != 1;

                            //_m1 = true;
                            //_m2 = true;
                        }
                        break;
                    case SystemMessage.GotEnoughPoints:
                    case SystemMessage.NotEnoughPoints:
                        {
                            _m1 = networkController.settingsGame.m_Map[3].status == 0;
                            _m2 = networkController.settingsGame.m_Map[4].status != 1;
                        }
                        break;
                    case SystemMessage.AddEarthSpirit:
                        {
                            _m1 = networkController.settingsGame.m_Map[4].status == 1;
                            _m2 = networkController.settingsGame.m_Map[5].status != 1;
                        }
                        break;

                    default:
                        {
                            _m1 = true;
                            _m2 = true;
                        }
                        break;
                }
            }

            catch { }

            if (!(_m1 && _m2))
            {
                return true; // Means that the message is NOT permitted to being started at the moment.
            }

            return false; // Got the permission to being started.
        }
        #endregion
    }
}