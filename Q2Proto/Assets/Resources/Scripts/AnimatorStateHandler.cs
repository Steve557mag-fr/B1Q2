using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorStateHandler : MonoBehaviour
{

    public UnityEvent onEnded;

    public void PlayMG()
    {
        GameManager.instance.ToggleCurtain(true);
        LeanTween.delayedCall(2f, GameManager.instance.GetCurrentMiniGame().GameSetup);
    }

    public void VerifyPath(int i)
    {
        ((SpaceTravel)GameManager.instance.GetCurrentMiniGame()).VerifyPath(i);
    }

}
