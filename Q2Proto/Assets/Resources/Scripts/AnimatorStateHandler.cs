using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorStateHandler : StateMachineBehaviour
{

    public UnityEvent onEnded;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onEnded.Invoke();
    }

    public void PlayMG()
    {
        GameManager.instance.PlayClap(GameManager.instance.currentMG);
    }

}
