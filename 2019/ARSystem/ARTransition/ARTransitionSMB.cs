using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARSystem;

public class ARTransitionSMB : StateMachineBehaviour
{
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.IsName(ARTransition.FADE_OUT_name) || stateInfo.IsName(ARTransition.FADE_IN_name))
		{
			ARMode.Instance._ARTransition.OnFinish();
		}

		//if (stateInfo.IsName("Death") || stateInfo.IsName("Recover")) //@TODO: create separate SMB for ARObjectAnimation
		//{
		//	//animator.gameObject ===> arObj.Animation.OnFinish();
		//	ARMode.Instance._ARTransition.OnFinish();
		//}
	}

	//public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//	if ((stateInfo.IsName("FadeOut") || stateInfo.IsName("FadeIn")) && stateInfo.normalizedTime >= 0.75f)
	//	{
	//		ARMode.SetActive(!ARMode.IsActivated, RunFadeOut);
	//	}
	//}
}