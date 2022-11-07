using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine.UI;
using System.Linq;

public class CharGenerationManager : MonoBehaviour
{
    public LinksManager linksScript;
    public GameObject[] CharGenerationSteps;

    [NonSerialized]
    public GameObject _clonedChosenBaseChar;

    private GameObject _clonedChosenBaseCharPrev;

    private const string CharGenBodyPartsPath = "";

    private float[] headSectionMode_posY = new float[] { 0f, -488f, 53.0206f, -85f };
    private float[] headSectionMode_scale = new float[] { 1.167375f, 2.688082f, 0.8034356f };

    private void Awake()
    {
        Messenger<int>.AddListener(CharGenStepsMessenger.CharGenStepChanged, OnCharGenStepChanged);
        linksScript.charGenDrawManager.InitializeColourDictForChar();
    }

    private void OnDestroy()
    {
        Messenger<int>.RemoveListener(CharGenStepsMessenger.CharGenStepChanged, OnCharGenStepChanged);
    }

    private void OnCharGenStepChanged(int value)
    {
        switch (value)
        {
            case 0: DisplayCharGenScreen02(); break;
            case 1: DisplayHideCharGenScreen03(); break;
            case 2: linksScript.charGenSaveLoadManager.SaveGeneratedChar(); break;
            case 3: FinishCharGeneration(); break;
        }
    }

    private void DisplayCharGenScreen02()
    {
        if (linksScript.dashboardManager.charPlaceHolders[0].GetComponentInChildren<Animator>() != null)
        {
            _clonedChosenBaseCharPrev = linksScript.dashboardManager.charPlaceHolders[0].GetComponentInChildren<Animator>().gameObject;
        }

        _clonedChosenBaseChar = linksScript.charGenDrawManager.CloneChosenBaseChar();
        linksScript.charGenDrawManager.InitChosenCharCacheLists(_clonedChosenBaseChar);

        if (linksScript.charGenDataManager.charGenCategoriesGeneralList.Count == 0 || _clonedChosenBaseChar.name != _clonedChosenBaseCharPrev.name)
        {
            linksScript.charGenDataManager.GetCharGenCategories(linksScript.charGenDataManager.currentCharRace, linksScript.charGenDataManager.currentCharGender);
        }
    }

    private void DisplayHideCharGenScreen03()
    {
        bool currActivity = linksScript.charGenEnvDrawManager.categoriesHUD[2].activeSelf;
        linksScript.charGenEnvDrawManager._categoriesGeneralContainer.SetActive(currActivity);

        linksScript.charGenEnvDrawManager.ShowHideCategoriesHUD(currActivity, 0, 2);

        ChangeCharScale(headSectionMode_posY[0], headSectionMode_scale[0]);

        int _placeHolderNum;

        if (currActivity == false)
        {
            linksScript.charGenEnvDrawManager.categoriesHUD[2].transform.Find("BtnStepBack").GetComponentInChildren<Button>().onClick.AddListener(() => DisplayHideCharGenScreen03());
            linksScript.charGenEnvDrawManager.categoriesHUD[2].GetComponentInChildren<Text>().text = "Имя персонажа";
            _placeHolderNum = 1;
        }
        else
        {
            if (linksScript.charGenEnvDrawManager.btnSliderChangeLook.value == 1)
            {
                ChangeCharScale(headSectionMode_posY[1], headSectionMode_scale[1]);
            }

            linksScript.charGenerationManager.CharGenerationSteps[1].SetActive(currActivity);
            linksScript.charGenerationManager.CharGenerationSteps[2].SetActive(!currActivity);
            linksScript.charGenEnvDrawManager.categoriesHUD[2].transform.Find("BtnStepBack").GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            linksScript.charGenEnvDrawManager.categoriesHUD[2].GetComponentInChildren<Text>().text = "";
            _placeHolderNum = 0;
        }

        RelocateChar(_placeHolderNum);

        Animator currAnim = _clonedChosenBaseChar.GetComponent<Animator>();
        currAnim.SetBool("goAnim", true);
    }

    public void StepBack(int currPanelIndex)
    {
        CharGenerationSteps[currPanelIndex].SetActive(false);
        CharGenerationSteps[currPanelIndex - 1].SetActive(true);

        if (currPanelIndex == 1)
        {
            Toggle activeToggle = linksScript.charGenEnvDrawManager.baseCharsCarouselProperties.charsToggleGroup.GetActive();
            activeToggle.isOn = false;
            activeToggle.isOn = true;
            linksScript.charGenDrawManager.ChosenCharPartsCacheGeneralDict.Clear();
            linksScript.charGenEnvDrawManager.btnSliderChangeLook.value = 0;
        }
    }

    public void StepForward(int currPanelIndex)
    {
        if (currPanelIndex < 2)
        {
            CharGenerationSteps[currPanelIndex + 1].SetActive(true);
        }

        Messenger<int>.Broadcast(CharGenStepsMessenger.CharGenStepChanged, currPanelIndex, MessengerMode.DONT_REQUIRE_LISTENER);

        if (currPanelIndex != 2 && currPanelIndex != 3)
        {
            CharGenerationSteps[currPanelIndex].SetActive(false);
        }

        else if (currPanelIndex == 3)
        {
            CharGenerationSteps[2].SetActive(false);
        }
    }

    public void ChangeBodyColour()
    {
        ChangeCharColour();
    }

    public void OnValueChangedGender(float newValue)
    {
        ChangeGender();
    }

    public void ChangeGender()
    {
        if (linksScript.charGenDrawManager.maleChars.Count > 0 && linksScript.charGenDrawManager.femaleChars.Count > 0)
        {
            bool _switch = !linksScript.charGenDrawManager.femaleChars[0].activeSelf;

            foreach (GameObject currFemaleChar in linksScript.charGenDrawManager.femaleChars)
            {
                currFemaleChar.SetActive(_switch);
            }

            foreach (GameObject currMaleChar in linksScript.charGenDrawManager.maleChars)
            {
                currMaleChar.SetActive(!_switch);
            }

            if (_switch)
            {
                int currMaleIndex = linksScript.charGenDrawManager.maleChars.FindIndex(x => x.GetComponent<Toggle>().isOn);
                linksScript.charGenDrawManager.femaleChars[currMaleIndex].GetComponent<Toggle>().isOn = true;

                foreach (GameObject currGO in linksScript.charGenDrawManager.maleChars)
                {
                    currGO.GetComponent<Toggle>().isOn = false;
                }
            }

            else
            {
                int currFemaleIndex = linksScript.charGenDrawManager.femaleChars.FindIndex(x => x.GetComponent<Toggle>().isOn);
                linksScript.charGenDrawManager.maleChars[currFemaleIndex].GetComponent<Toggle>().isOn = true;

                foreach (GameObject currGO in linksScript.charGenDrawManager.femaleChars)
                {
                    currGO.GetComponent<Toggle>().isOn = false;
                }
            }
        }
    }

    public void ToggleRaceOnValueChanged(bool newValue)
    {
        if (newValue)
        {
            Toggle activeToggle = linksScript.charGenEnvDrawManager.baseCharsCarouselProperties.charsToggleGroup.GetActive();
            Animator currAnim = activeToggle.gameObject.GetComponent<Animator>();
            currAnim.SetBool("goAnim", true);

            if (linksScript.charGenDrawManager.prevChar != null && linksScript.charGenDrawManager.prevChar != activeToggle.gameObject)
            {
                Animator prevAnim = linksScript.charGenDrawManager.prevChar.GetComponent<Animator>();
                prevAnim.SetBool("goAnim", false);
                linksScript.charGenDrawManager.prevChar = activeToggle.gameObject;
            }
        }
    }

    public void OnValueChangedSection(float newValue)
    {
        ChangeSection(newValue);
    }

    private void ChangeSection(float newValue)
    {
        for (int i = 0; i < 3; i++)
        {
            linksScript.charGenEnvDrawManager.charGenSectionsListObj[i].SetActive(false);
        }

        linksScript.charGenEnvDrawManager.charGenSectionsListObj[(int)newValue].SetActive(true);

        if (newValue == 1)
        {
            ChangeCharScale(headSectionMode_posY[1], headSectionMode_scale[1]);
        }
        else
        {
            ChangeCharScale(headSectionMode_posY[0], headSectionMode_scale[0]);
        }
    }

    private void ChangeCharScale(float _posY, float _scale)
    {
        GameObject _char = GameObject.FindGameObjectWithTag("CharGenInstance");
        RectTransform currRect = _char.GetComponent<RectTransform>();
        currRect.localScale = new Vector3(_scale, _scale, _scale);
        currRect.localPosition = new Vector3(0f, _posY, 0f);
    }

    public void ChangeCharBodyPart(string bodyPartKey, string CategoryKey, Image _toggleImg)
    {
        if (_toggleImg.sprite == linksScript.charGenEnvDrawManager.subCatSprites[1])
        {
            return;
        }

        string _path = CharGenBodyPartsPath + linksScript.charGenDataManager.currentCharRace + "/" + CategoryKey + "/" + linksScript.charGenDataManager.currentCharRace + "_" + CategoryKey + "_atlas";
        Sprite[] currSprites = Resources.LoadAll<Sprite>(_path);
        Sprite[] newSprites = Array.FindAll(currSprites, item => (item.name.Contains(bodyPartKey) && item.name.Split('_')[0].ToLower() == linksScript.charGenDataManager.currentCharGender));

        foreach (Image currImgPart in linksScript.charGenDrawManager.ChosenCharPartsCacheGeneralDict[CategoryKey.ToLower()])
        {
            Sprite _freshSprite;
            string _currCharPartName = currImgPart.name;
            string[] _currCharPartNameArray = _currCharPartName.Split('_');

            string[] pluralParts = { "Legs", "Arms" , "Torso", "Eyes"};

            if (Array.Find(pluralParts, item => item == CategoryKey) != null)
            {
                _freshSprite = Array.Find(newSprites, item => (item.name.Contains(_currCharPartNameArray[1]) && item.name.Contains(_currCharPartNameArray[3])));
            }
            else
            {
                _freshSprite = Array.Find(newSprites, item => (item.name.Contains(_currCharPartNameArray[1])));
            }

            currImgPart.sprite = _freshSprite;
        }

        linksScript.charGenEnvDrawManager.SwapSpriteSubCatBtn(_toggleImg);
    }

    public void ChangeCharColour()
    {
        if (linksScript.charGenDrawManager.charPartsColoring.Count != 0)
        {
            int currIndex = linksScript.charGenDrawManager.colourListForChar.IndexOf(linksScript.charGenDrawManager.currCharColour);

            if (currIndex < linksScript.charGenDrawManager.colourListForChar.Count - 1)
            {
                currIndex++;
                linksScript.charGenDrawManager.currCharColour = linksScript.charGenDrawManager.colourListForChar[currIndex];
            }
            else if (currIndex == linksScript.charGenDrawManager.colourListForChar.Count - 1)
            {
                linksScript.charGenDrawManager.currCharColour = linksScript.charGenDrawManager.colourListForChar[0];
            }

            foreach (Image currImgPart in linksScript.charGenDrawManager.charPartsColoring)
            {
                currImgPart.color = linksScript.charGenDrawManager.currCharColour;
            }
        }
    }

    public void RelocateChar(int _placeHolderNum)
    {
        Transform newCharHolderTr = linksScript.dashboardManager.charPlaceHolders[_placeHolderNum].transform;
        _clonedChosenBaseChar.transform.SetParent(newCharHolderTr, false);
    }

    public void CreateAptBtnForChar()
    {
        Destroy(_clonedChosenBaseChar.GetComponent<Toggle>());

        GameObject newGO = new GameObject();
        newGO.transform.SetParent(_clonedChosenBaseChar.transform, false);
        newGO.name = "_BtnApartment";
        newGO.AddComponent<Button>().onClick.AddListener(linksScript.dashboardManager.ShowAptPanel);
        newGO.GetComponent<Button>().transition = Selectable.Transition.None;

        RectTransform _rect = newGO.AddComponent<RectTransform>();
        _rect.localPosition = new Vector3(0, -48.73f, 0);
        _rect.localScale = new Vector3(1f, 1f, 1f);
        _rect.sizeDelta = new Vector2(375, 750);
        Image image = newGO.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0);

        newGO.GetComponent<Button>().enabled = false;
    }

    private void FinishCharGeneration()
    {
        linksScript.dashboardManager.ShowDashboardViaCoroutine();
        RelocateChar(2);
        ChangeCharScale(headSectionMode_posY[0], headSectionMode_scale[2]);
        _clonedChosenBaseChar.GetComponentInChildren<Button>().enabled = true;
        GameObject.FindGameObjectWithTag("_CharGenerationPanel").SetActive(false);
        linksScript.charGenerationManager.gameObject.SetActive(false);
    }
}



