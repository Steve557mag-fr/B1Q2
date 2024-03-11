using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame : MonoBehaviour
{
    public string tileText = "";
    public string descriptionText = "";
    
    [HideInInspector] public bool isEnded = false;

    [HideInInspector] public GameObject gArea;

    public virtual void StartGame(GameObject area) { gArea = area; }
    public virtual void UpdateGame() {}
    public virtual void RestartGame() { GameManager.GameOver(); }
    public virtual void WinGame() { FindAnyObjectByType<GameManager>().NextMiniGame(); }

}
