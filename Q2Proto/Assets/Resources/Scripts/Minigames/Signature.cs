using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signature : MonoBehaviour
{
    [Header("Game Settings")]
    public People[] peoples;



}

public struct People
{
    public bool isInteracted;
    public bool isEvil;
    public CutoutBehaviour cutout;
}