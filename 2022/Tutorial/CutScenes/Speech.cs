using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Sirenix.OdinInspector;

namespace TutorialActions
{
    public class FromUnitConfig : ISpeechActor
    {
        [SerializeField] UnitConfig Config;

        public ISpeechActor Actor => this;
        public string HeroName => Config.Caption;
        public CutSceneHeroView CutSceneHeroView => null;
        public Sprite IconUI => Config.Icon;
        public Unit Unit => null;
        public bool TypeText => true;

        public void OnStart()
        {
            var c = Unit as ISpeechActor;
            if(c != null)
            {
                c.OnStart();
            }
        }

        public void OnStop()
        {
            var c = Unit as ISpeechActor;
            if (c != null)
            {
                c.OnStop();
            }
        }

        public void OnStopType()
        {
            var c = Unit as ISpeechActor;
            if (c != null)
            {
                c.OnStopType();
            }
        }
    }

    public class GenericSpeech : ISpeechActor
    {

        [SerializeField] Sprite Icon;
        [SerializeField] string NameKey;

        public Unit Unit { get { return null; } }
        public CutSceneHeroView CutSceneHeroView { get { return null; } }

        public string HeroName { get { return Localization.Get(NameKey); } }

        public Sprite IconUI { get { return Icon; } }

        public bool TypeText => false;

        public void OnStart() { }

        public void OnStop() { }

        public void OnStopType() { }
    }

    /*
    public class FromUnitSelector : ISpeechActor
    {
        [SerializeField] Selectors.IUnitSelector Unit;

        public ISpeechActor Actor { get { return Unit; } }
        public string HeroName { get { return Unit != null ? Localization.Get(Unit.Visual.NickKey) : ""; } }

        public CutSceneHeroView CutSceneHeroView { get { return null; } }

        BattleSession Session { get { return BattleSession.ActiveSession; } }

        public Sprite IconUI
        {
            get
            {
                if (Unit != null)
                {
                    return Unit.IconUI;
                }
                return null;
            }
        }

        public bool TypeText { get { return true; } }

        public Unit Unit
        {
            get
            {
                var hero = Unit.Select(EffectorContext.Create(Session), Session.Battle.Units).FirstOrDefault() as Unit;
                if (hero != null)
                {
                    return hero;
                }
                return null;
            }
        }

        public void OnStart()
        {
            var c = Unit as ISpeechActor;
            if (c != null)
            {
                c.OnStart();
            }
        }

        public void OnStop()
        {
            var c = Unit as ISpeechActor;
            if (c != null)
            {
                c.OnStop();
            }
        }

        public void OnStopType()
        {
            var c = Unit as ISpeechActor;
            if (c != null)
            {
                c.OnStopType();
            }
        }
    }
    */

    public class CutSceneSpeechActor : ISpeechActor
    {
		[SerializeField] protected string Name = "Mentor";
        public string HeroName =>  Localization.Get(string.Format("TUTORIAL_HERO_{0}", Name));
        public bool TypeText => false;
        public Sprite IconUI => null;
        public CutSceneHeroView CutSceneHeroView => Game.Config.Meta.UI.CutSceneHeroViews.GetOrDefault(Name);
        public Unit Unit => null;

        public void OnStart() { }
        public void OnStop() { }
        public void OnStopType() { }
    }

    public interface ISpeechActor
    {
		Sprite IconUI { get; }
        string HeroName { get; }
        bool TypeText { get; }
        CutSceneHeroView CutSceneHeroView { get; }
		Unit Unit { get; }
        void OnStart();
        void OnStop();
        void OnStopType();
    }

	public class Speech
	{
		public ISpeechActor Actor;
        
        public enum Anchors
		{
			Center,
			Left,
            Right,
            UpCenter,
		}

		[SerializeField] public bool ShowExtraOptions = false;
		[ShowIf("ShowExtraOptions")] public Anchors Anchor = Anchors.Left;
        [ShowIf("ShowExtraOptions")] public bool UseDarkBackground;
        [ShowIf("ShowExtraOptions"), SerializeField] bool Hint;
		[ShowIf("ShowExtraOptions")] public Vector2 Position;
        [ShowIf("ShowExtraOptions")] public bool HideClickToContinue = true;
		
        public LocalizedString Message;
		public bool IsHint { get { return UseDarkBackground || Hint; }}
	}

}
