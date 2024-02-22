using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TutorialActions;
using TMPro;
using System;
using System.Linq;

public class HeroSpeechWindow : PendingUI
{
    [SerializeField] private SpeechTypeConfig SpeechTypeConfig;
    [SerializeField] private Transform HeroPosition;
    [SerializeField] private TextInfo Hint;
    [SerializeField] private TextInfo Subtitle;
    [SerializeField] bool DebugTestTyping;
    ISpeechActor Actor;
    Coroutine TextAppearCoroutine;
    public event Action OnContinueAction;

    [System.Serializable]
    class TextInfo
    {
        public RectTransform Holder = null;
        public TextMeshProUGUI Text = null;
        public GameObject ClickToContinue;
        public Button ClickToContinueButton = null;
    }

    private CutSceneHeroView Hero;
    private System.Action Action;
    private bool IsHint;

    TextInfo ActiveText { get { return IsHint ? Hint : Subtitle; } }

    public string SpeechText
    {
        get { return ActiveText.Text.text; }
        set { ActiveText.Text.text = value; ActiveText.Text.maxVisibleCharacters = ActiveText.Text.text.Length; }
    }

    protected override void OnEnable()
    {
        if (!IsHint && Actor != null)
        {
            if (TextAppearCoroutine == null)
            {
                TextAppearCoroutine = StartCoroutine(ShowText());
            }
        }
    }

    protected override void OnDestroy()
    {
        if (Actor != null)
        {
            Actor.OnStop();
            Actor = null;
        }
    }

    public void Hide()
    {
        if (Actor != null)
        {
            Actor.OnStop();
        }
        gameObject.SetActive(false);
    }

    public void Close()
    {
        if (Actor != null)
        {
            Actor.OnStop();
            Actor = null;
        }
        Destroy(gameObject);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        if (Actor != null)
        {
            Actor.OnStop();
        }
    }

    public void Activate(Speech speech, Transform place, bool hint, System.Action continueAction = null, bool closable = true)
    {
        if (Actor != null)
        {
            Actor.OnStop();
        }
        Actor = speech.Actor;
        if (Actor != null)
        {
            Actor.OnStart();
        }

        Action = continueAction;
        IsHint = hint;
        Hint.Holder.gameObject.SetActive(IsHint);
        Subtitle.Holder.gameObject.SetActive(!IsHint);
        ActiveText.ClickToContinue.SetActive(closable && !speech.HideClickToContinue);
        if (ActiveText.ClickToContinueButton != null && continueAction != null && closable)
        {
            ActiveText.ClickToContinueButton.gameObject.SetActive(true);
            ActiveText.ClickToContinueButton.onClick.RemoveAllListeners();
            ActiveText.ClickToContinueButton.onClick.AddListener(OnContinueActionHandled);
        }

        gameObject.SetActive(true);

        if (place)
        {
            transform.SetParent(place, false);
            transform.localPosition = speech.Position;
        }
        if (!Hero)
        {
            var prefab = Game.Config.Meta.UI.CutSceneHeroViews.GetOrDefault("Mentor");
            if (speech.Actor != null && speech.Actor.CutSceneHeroView != null)
            {
                prefab = speech.Actor.CutSceneHeroView;
            }
            Hero = Instantiate(prefab);
        }
        Hero.Activate(HeroPosition, speech.Actor == null ? null : speech.Actor.IconUI);

        var pivot = new Vector2(0, 0);
        if (speech.Anchor == Speech.Anchors.Center || speech.Anchor == Speech.Anchors.UpCenter)
        {
            pivot = new Vector2(0.5f, 0);
        }
        var direction = (speech.Anchor == Speech.Anchors.Right ? -1 : 1);
        var s = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
        transform.localScale = s;

        s = new Vector3(Mathf.Abs(ActiveText.Text.transform.localScale.x) * direction, ActiveText.Text.transform.localScale.y, ActiveText.Text.transform.localScale.z);
        ActiveText.Text.transform.localScale = s;
        ActiveText.Text.text = speech.Message;

        ActiveText.Holder.pivot = pivot;
        ActiveText.Holder.transform.localPosition = Vector3.zero;

        if (IsHint || speech.Actor == null || !speech.Actor.TypeText)
        {
            ActiveText.Text.maxVisibleCharacters = ActiveText.Text.text.Length;
        }
        else
        {
            if (gameObject.activeInHierarchy)
            {
                TextAppearCoroutine = StartCoroutine(ShowText());
            }
        }
    }

    bool StopTyping()
    {
        if (Actor != null)
        {
            Actor.OnStopType();
        }
        if (TextAppearCoroutine != null)
        {
            StopCoroutine(TextAppearCoroutine);
            TextAppearCoroutine = null;
            ActiveText.Text.maxVisibleCharacters = ActiveText.Text.text.Length;
            return true;
        }
        return false;
    }

    IEnumerator ShowText()
    {
        var text = ActiveText.Text;
        text.maxVisibleCharacters = 0;
        var time = Time.timeSinceLevelLoad;
        while (text.maxVisibleCharacters < text.text.Length)
        {
            var letter = text.text[text.maxVisibleCharacters];
            var pause = SpeechTypeConfig.Pauses.GetOrDefault(letter.ToString(), SpeechTypeConfig.DefaultPause);
            if (pause.Pause > 0)
            {
                time += pause.Pause;
                text.maxVisibleCharacters++;
                if (text.maxVisibleCharacters < text.text.Length && pause.SkipNext.Length > 0)
                {
                    var skipString = pause.SkipNext.FirstOrDefault(s => text.maxVisibleCharacters + s.Length < text.text.Length && text.text.IndexOf(s, text.maxVisibleCharacters, s.Length) != -1);
                    if (skipString != null)
                    {
                        text.maxVisibleCharacters += skipString.Length;
                    }
                }
            }
            while (time > Time.timeSinceLevelLoad)
            {
                yield return null;
            }
        }
        TextAppearCoroutine = null;
        StopTyping();
    }

    void OnContinueActionHandled()
    {
        if (StopTyping())
        {
            return;
        }
        if (DebugTestTyping && Debug.isDebugBuild)
        {
            TextAppearCoroutine = StartCoroutine(ShowText());
            return;
        }
        var a = Action;
        Action = null;
        a?.Invoke();
        OnContinueAction?.Invoke();
    }

    public void OnStartAction()
    {
        var a = Action;
        Action = null;
        a?.Invoke();
    }


    private void Update()
    {
        if (ActiveText.ClickToContinueButton != null && ActiveText.ClickToContinueButton.gameObject.activeInHierarchy)
        {
            ActiveText.ClickToContinueButton.onClick.Invoke();
        }
    }
}