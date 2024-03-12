using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MiniGame : MonoBehaviour
{
    [Header("Clap Informations")]
    public string tileText = "";
    public string descriptionText = "";

    [Header("Animations Settings")]
    public PlayableDirector winSequence;
    public PlayableDirector looseSequence;

    [Header("Decoration")]
    public float timerShowHide = 0.5f;
    public float outScreenDecorationY = -5;
    public GameObject decorationLayer;

    [HideInInspector] public bool isEnded = false;
    [HideInInspector] public GameObject gArea;

    public virtual void StartGame(GameObject area) {
        LeanTween.moveLocal(decorationLayer, new Vector3(0, 0, 0), timerShowHide);
        gArea = area;
    }
    
    public virtual void UpdateGame() {}
    
    public virtual void LooseGame(PlayableDirector obj)
    {
        LeanTween.moveLocal(decorationLayer, new Vector3(0, outScreenDecorationY, 0), timerShowHide).setOnComplete(() =>
        {
            GameManager.GetInstance().GameOver();
        });
    }

    public virtual void WinGame() {
        ClearGame();
        winSequence.Play();
        winSequence.stopped += (PlayableDirector o) => {
            LeanTween.moveLocal(decorationLayer, new Vector3(0, outScreenDecorationY, 0), timerShowHide);
            GameManager.GetInstance().NextMiniGame();
        };
    }

    public virtual void ClearGame()
    {
        foreach (CutoutBehaviour enemy in gArea.GetComponentsInChildren<CutoutBehaviour>())
        {
            Destroy(enemy.gameObject);
        }
    }

}
