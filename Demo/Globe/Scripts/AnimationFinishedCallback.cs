using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFinishedCallback : StateMachineBehaviour {

    public string TrueWhenFinished;

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(TrueWhenFinished, false);
	}

}
