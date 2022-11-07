using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace TutorialEventSystem
{
    public class TutorialCrashRecovery : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.activeSceneChanged += MainStuff;
            StartCoroutine("MainStuffCoroutine");
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= MainStuff;
        }

        void MainStuff(Scene prevScene, Scene currScene)
        {
            StartCoroutine("MainStuffCoroutine");
        }

        IEnumerator MainStuffCoroutine()
        {
            if (SceneManager.GetActiveScene().name != "Scene_Offline") yield break; // get out of here if we are not on the Map scene

            if (!TutorialEventController.Singleton)
            {
                Debug.LogError("TutorialEventSystem.Singleton == null. Crash Recovery has been skipped!");
                yield break;
            }

            TutorialPageLink _data;
            _data.page = null;

            if (PlayerPrefs.HasKey(TutorialExtensions.tPageIdToRecoverAfterCrash))
            {
                int tPageId = PlayerPrefs.GetInt(TutorialExtensions.tPageIdToRecoverAfterCrash);
                _data = TutorialEventController.Singleton.PageListData.pageLinkList.Find(m => (m.page != null && m.page.IsRecoverableAfterCrash() && m.page.GetInstanceID() == tPageId));
            }

            if (_data.page)
            {
                while (true)
                {
                    if (LoadingScreenController.Singleton && LoadingScreenController.Singleton.IsCurrentMapLoaded())
                    { break; }

                    yield return null;
                }

                if (_data.page.OnEvent == SystemMessage.ImproveAirSpirit)
                {
                    // RESTORE [power] and [will] of AirSpirit to the initial values
                    SettingsOfflineUI.Soul airSoul = TutorialEventController.Singleton.networkController.settingsGame.m_Souls.Find(s => s.id == 0);
                    if (airSoul != null)
                    {
                        airSoul.power_current = 286;
                        airSoul.will_current = 264;
                    }
                }
                else if (_data.page.OnEvent == SystemMessage.OpenSpellChest)
                {
                    //
                    // RESTORE [ElectricArrow] to the initial state !!!
                    //
                }
                else if (_data.page.OnEvent == SystemMessage.RevealMapCell0105)
                {
                    //
                    // RESTORE [EarthSpirit] to the unchosen state !!!
                    //
                }

                TutorialEventController.Singleton.RunEvent(_data.page.OnEvent);
            }
        }
    }
}
