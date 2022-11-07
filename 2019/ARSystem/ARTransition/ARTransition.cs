using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARTransition
	{
		#region Variables
		public static readonly string FADE_IN_name;
		public static readonly string FADE_OUT_name;

		private readonly int FADE_IN;
		private readonly int FADE_OUT;

		private Animator animator;

		private Action onFinishCallback;
		#endregion

		//---------------------------------------------------------

		static ARTransition()
		{
			FADE_OUT_name = "FadeIn";
			FADE_IN_name = "FadeOut";
		}

		public ARTransition(Animator _animator)
		{
			FADE_IN = Animator.StringToHash(FADE_OUT_name);
			FADE_OUT = Animator.StringToHash(FADE_IN_name);

			animator = _animator;
		}

		public void Play(bool flag, Action callback = null)
		{
			animator?.SetTrigger(flag ? FADE_IN : FADE_OUT);
			SetOnFinishCallback(callback);
		}

		public void OnFinish()
		{
			onFinishCallback?.Invoke();
			SetOnFinishCallback(null);
		}

		//---------------------------------------------------------

		private void SetOnFinishCallback(Action callback)
		{
			onFinishCallback = callback;
		}
	}
}