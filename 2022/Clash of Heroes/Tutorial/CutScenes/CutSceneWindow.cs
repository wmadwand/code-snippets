using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TutorialActions;

public class CutSceneWindow : ModalWindow
{
    [SerializeField] protected Transform Left;
    [SerializeField] protected Transform Right;
	[SerializeField] protected Transform Center;
    [SerializeField] protected Transform UpCenter;
    [SerializeField] protected GameObject ClickCollider;

    private Speech[] Conversation;
    private int CurrentSpeech = 0;

    private Dictionary<string, HeroSpeechWindow> Heroes;

    public event System.Action OnComplete;

    private HeroSpeechWindow LeftHero;
    private HeroSpeechWindow RightHero;
    private bool Closable;
    private GameObject DarkBackground;

    const string DefaultHeroName = "Dictor";

    protected override void Awake()
    {
        base.Awake();
        if (UseDarkBackground)
        {
            DarkBackground = transform.GetChild(0).gameObject;
        }
        Heroes = new Dictionary<string, HeroSpeechWindow>();
    }

    public void Init(Speech speech, bool closable = true)
    {
        Init(new Speech[] { speech }, closable);
        if (!closable)
        {
            OnNext(closable);
        }
    }

    public void Init(Speech[] conversation, bool closable = true)
    {
        this.Closable = closable;
        this.Conversation = conversation;
        foreach (var s in conversation)
        {
            var heroName = DefaultHeroName;
            if(s.Actor != null)
            {
                heroName = s.Actor.HeroName;
            }
            if (Heroes.ContainsKey(heroName))
            {
                continue;
            }
            var hero = Instantiate(Game.Config.Meta.UI.HeroSpeechWindow);
			hero.name = string.Format("CutSceneWindow-{0}", s.Message.Key);
			if (Center.transform.childCount == 0 && s.Anchor == Speech.Anchors.Center)
            {
                hero.Deactivate();
            }
			else if (Left.transform.childCount == 0 && s.Anchor == Speech.Anchors.Left)
            {
                hero.Deactivate();
            }
			else if (Right.transform.childCount == 0 && s.Anchor == Speech.Anchors.Right)
            {
                hero.Deactivate();
            }
            else
            {
                hero.Hide();
            }
            Heroes.Add(heroName, hero);
        }
        OnNext(closable);
    }

	public void OnNext(bool closable = true)
    {
        if (DarkBackground) DarkBackground.SetActive(false);
        if (Conversation.Length == CurrentSpeech)
        {
            OnComplete?.Invoke();
            OnComplete = null;
            if (Closable)
            {
                DoClose();
            }
            else if (ClickCollider)
            {
                Destroy(ClickCollider);
            }
            return;
        }

        var speech = Conversation[CurrentSpeech];


        var currentHero = Heroes[speech.Actor == null ? DefaultHeroName : speech.Actor.HeroName];
		if (speech.Anchor != Speech.Anchors.Right)
        {
            if (LeftHero != null && LeftHero != currentHero)
            {
                LeftHero.Hide();
            }
            LeftHero = currentHero;
            if (RightHero != null)
            {
                RightHero.Deactivate();
            }

        }
        else
        {
            if (RightHero != null && RightHero != currentHero)
            {
                RightHero.Hide();
            }
            RightHero = currentHero;
            if (LeftHero != null)
            {
                LeftHero.Deactivate();
            }
        }
		Transform parent;
		switch(speech.Anchor) {
			case Speech.Anchors.Center:
				parent = Center;
				break;
			case Speech.Anchors.Left:
				parent = Left;
                break;
			case Speech.Anchors.Right:
				parent = Right;
                break;
            case Speech.Anchors.UpCenter:
                parent = UpCenter;
                break;
			default:
				parent = Center;
                break;
		}
        UseDarkBackground = speech.UseDarkBackground;
        currentHero.Activate(speech, parent, speech.IsHint, () => { OnNext(closable); }, closable);
        if (DarkBackground) DarkBackground.SetActive(UseDarkBackground);
        CurrentSpeech++;
    }

}
