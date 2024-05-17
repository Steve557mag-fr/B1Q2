using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public KeyCode Action1, Action2, Action3;

    public KeyCode[] Actions { get { return new KeyCode[] { Action1, Action2, Action3 }; } }

    public static int GetAxis(KeyCode A, KeyCode B)
    {
        return System.Convert.ToInt16(Input.GetKey(A)) - System.Convert.ToInt16(Input.GetKey(B));
    }

    public static int GetAxisDown(KeyCode A, KeyCode B)
    {
        return System.Convert.ToInt16(Input.GetKeyDown(A)) - System.Convert.ToInt16(Input.GetKeyDown(B));
    }
    
    public static Player Get()
    {
        return FindAnyObjectByType<Player>();
    }

}
