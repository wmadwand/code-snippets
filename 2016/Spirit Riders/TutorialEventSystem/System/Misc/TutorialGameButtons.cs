using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TutorialEventSystem
{
    public delegate GameObject GameButtonHandler();

    public class TutorialGameButtons : MonoBehaviour
    {
        #region Singleton
        private static TutorialGameButtons _singleton;
        public static TutorialGameButtons Singleton
        {
            get
            {
                if (!_singleton)
                {
                    _singleton = FindObjectOfType<TutorialGameButtons>();
                }

                return _singleton;
            }
        }
        #endregion

        [SerializeField]
        GameObject dummyClickButton;
        private GameObject currButtonPanel;

        private Dictionary<GameButton, GameButtonHandler> gameButtons;

        #region Basic buttons
        public GameObject None()
        {
            return dummyClickButton;
        }

        private GameObject PlayGame()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#PlayGameButton").gameObject : null;
        }

        private GameObject Bonus()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("PanelFinish");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#BonusButton").gameObject : null;
        }
        #endregion

        #region Spirit menu buttons
        private GameObject SpiritMenu()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#SpiritMenuButton").gameObject : null;
        }

        private GameObject ChooseAirSpirit()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ChooseAirSpiritButton").gameObject : null;
        }

        private GameObject TweakAirSpirit()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#TweakAirSpiritButton").gameObject : null;
        }

        private GameObject ChooseEarthSpirit()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ChooseEarthSpiritButton").gameObject : null;
        }

        private GameObject TweakEarthSpirit()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#TweakEarthSpiritButton").gameObject : null;
        }

        private GameObject SpiritPower()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#PowerButton").gameObject : null;
        }

        private GameObject SpiritWill()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#WillButton").gameObject : null;
        }

        private GameObject SpellSection()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#SpellSectionButton").gameObject : null;
        }
        #endregion

        #region HUD buttons
        private GameObject UseWeapon()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("Canvas");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ButtonWeapon").gameObject : null;
        }

        private GameObject UseAbility()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("Canvas");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ButtonAbility").gameObject : null;
        }

        private GameObject Jump()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("Canvas");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ButtonJump").gameObject : null;
        }
        #endregion

        #region Map cells buttons
        private GameObject MapCell0102()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#Island_02").gameObject : null;
        }

        private GameObject MapCell0103()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#Island_03").gameObject : null;
        }

        private GameObject MapCell0104()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#Island_04").gameObject : null;
        }
        #endregion

        #region Map stuff buttons
        private GameObject OpenSpellChest()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ButtonGetBonus04").gameObject : null;
        }

        private GameObject CollectEther()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#Farm05").gameObject : null;
        }

        private GameObject CollectSpell01()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#Island_03").gameObject : null;
        }
        #endregion

        #region Test buttons
        private GameObject Test1()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ButtonTest1").gameObject : null;
        }

        private GameObject Test2()
        {
            currButtonPanel = GameObject.FindGameObjectWithTag("UI");
            return currButtonPanel != null ? currButtonPanel.transform.FindDeepChild("#ButtonTest2").gameObject : null;
        }
        #endregion
        
        private void Awake()
        {
            _singleton = this;

            gameButtons = new Dictionary<GameButton, GameButtonHandler>();

            gameButtons[GameButton.None] = None;
            gameButtons[GameButton.TwoFingersTap] = None;

            gameButtons[GameButton.PlayGame] = PlayGame;
            gameButtons[GameButton.Bonus] = Bonus;

            gameButtons[GameButton.SpiritMenu] = SpiritMenu;
            gameButtons[GameButton.ChooseAirSpirit] = ChooseAirSpirit;
            gameButtons[GameButton.TweakAirSpirit] = TweakAirSpirit;

            gameButtons[GameButton.ChooseEarthSpirit] = ChooseEarthSpirit;
            gameButtons[GameButton.TweakEarthSpirit] = TweakEarthSpirit;

            gameButtons[GameButton.SpiritPower] = SpiritPower;
            gameButtons[GameButton.SpiritWill] = SpiritWill;
            gameButtons[GameButton.SpellSection] = SpellSection;

            gameButtons[GameButton.UseWeapon] = UseWeapon;
            gameButtons[GameButton.UseAbility] = UseAbility;
            gameButtons[GameButton.Jump] = Jump;

            gameButtons[GameButton.MapCell0102] = MapCell0102;
            gameButtons[GameButton.MapCell0103] = MapCell0103;
            gameButtons[GameButton.MapCell0104] = MapCell0104;

            gameButtons[GameButton.OpenSpellChest] = OpenSpellChest;
            gameButtons[GameButton.CollectEther] = CollectEther;
            gameButtons[GameButton.CollectSpell01] = CollectSpell01;

            gameButtons[GameButton.Test1] = Test1;
            gameButtons[GameButton.Test2] = Test2;
        }

        public GameObject FindButton(GameButton btn)
        {
            return gameButtons[btn]();
        }

        public void ClickButton(GameButton btn, GameObject btnGO = null)
        {
            GameObject currBtnGO = btnGO != null ? btnGO : gameButtons[btn]();

            if (!currBtnGO)
            {
                Debug.LogError("Proceed button not found on the Scene and has not been clicked!");
                return;
            }

            switch (btn)
            {
                case GameButton.UseAbility: ExecuteEvents.Execute(currBtnGO, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler); break;
                default: ExecuteEvents.Execute(currBtnGO, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler); break;
            }
        }
    }
}