using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorUtils : MonoBehaviour
{

    public static void CompleteStop(PlayableDirector director)
    {
        director.Stop();
        director.time = 0;
        director.Evaluate();
    }

}
