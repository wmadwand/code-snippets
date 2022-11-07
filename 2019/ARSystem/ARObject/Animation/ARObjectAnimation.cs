using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARObjectAnimation
{
	#region Variables
	public static readonly string IDLE_name;
	public static readonly string DEATH_name;
	public static readonly string RECOVER_name;

	public static readonly int IDLE;
	public static readonly int DEATH;
	public static readonly int RECOVER;

	private int currentState;
	private Animator animator;
	private Action onFinishCallback;
	#endregion

	//---------------------------------------------------------

	static ARObjectAnimation()
	{
		IDLE = Animator.StringToHash("Idle");
		DEATH = Animator.StringToHash("Death");
		RECOVER = Animator.StringToHash("Recover");
	}

	public ARObjectAnimation(Animator animator)
	{
		this.animator = animator;
	}

	public void Play(int state, Action callback = null)
	{
		animator?.SetTrigger(state);
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