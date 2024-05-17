using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateHandler : MonoBehaviour
{

    public void PlayMG() {
        GameManager.Get().SetCurtain(true, GameManager.Get().GetCurrentMG().GameSetup);
    }

}
