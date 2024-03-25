using System;
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

    [HideInInspector] public GameObject gArea;
    [HideInInspector] public bool softLockMiniGame = true;
    bool alreadyLinked = false;


    public virtual void StartGame(GameObject area) {
        LeanTween.moveLocal(decorationLayer, new Vector3(0, 0, 0), timerShowHide);
        softLockMiniGame = false;
        gArea = area;

        if (alreadyLinked) return;
        alreadyLinked = true;

        looseSequence.stopped += (PlayableDirector e) =>
        {
            DispearScene(() => {
                looseSequence.Stop();
                looseSequence.time = 0;
                looseSequence.Evaluate();
                GameManager.GetInstance().GameOver();
            });
        };

        winSequence.stopped += (PlayableDirector e) =>
        {
            DispearScene(() =>
            {
                winSequence.Stop();
                winSequence.time = 0;
                winSequence.Evaluate();
                GameManager.GetInstance().NextMiniGame();
            });
        };

    }

    Action<PlayableDirector> DispearScene(Action after)
    {
        LeanTween.moveLocal(decorationLayer, new Vector3(0, outScreenDecorationY, 0), timerShowHide)
            .setOnComplete(after);
        return null;
    }

    public virtual void UpdateGame() {
        if (softLockMiniGame) return;
    }
    
    public virtual void LooseGame(PlayableDirector obj)
    {
        if (softLockMiniGame) return;
        softLockMiniGame = true;

        Debug.Log($"[MG_{tileText}]: LOOSE MG");
        ClearGame();
        looseSequence.gameObject.SetActive(true);
        looseSequence.Play();
    }

    public virtual void WinGame() {
        if (softLockMiniGame) return;
        softLockMiniGame = true;

        Debug.Log($"[MG_{tileText}]: WIN MG");
        ClearGame();
        winSequence.gameObject.SetActive(true);
        winSequence.Play();
    }

    public virtual void ClearGame()
    {
        foreach (CutoutBehaviour enemy in gArea.GetComponentsInChildren<CutoutBehaviour>())
        {
            Destroy(enemy.gameObject);
        }
    }

}
