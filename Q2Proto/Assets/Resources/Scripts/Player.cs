using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public KeyCode[] keys;
    public bool isFreeze;

    public static int GetAxisDown(KeyCode A, KeyCode B) { return System.Convert.ToInt16(Input.GetKeyDown(A)) - System.Convert.ToInt16(Input.GetKeyDown(B)); }
    public static int GetAxis(KeyCode A, KeyCode B) { return System.Convert.ToInt16(Input.GetKey(A)) - System.Convert.ToInt16(Input.GetKey(B)); }

    private void Update()
    {
        if (isFreeze) return;



    }

    public static Player instance
    {
        get { return FindAnyObjectByType<Player>(); }
    }

}
