using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateHandler : MonoBehaviour
{

    public void PlayMG() {
        GameManager.Get().SetCurtain(true, GameManager.Get().GetCurrentMG().GameSetup);
    }

    public void VerifyPath(int index)
    {
        ((SpaceTraver)GameManager.Get().GetCurrentMG()).VerifyAnswer(index);
    }
}
