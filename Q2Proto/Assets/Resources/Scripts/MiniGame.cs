using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class MiniGame : MonoBehaviour
{
    [Header("Clap Settings")]
    public string title;
    public string description;

    [Header("Sequences")]
    public PlayableDirector seqWin;
    public PlayableDirector seqLoose;

    [Header("Events")]
    public UnityEvent onBegin, onEnded;

    [Header("Layers")]
    public float layerMoveTime = 0.5f;
    public LeanTweenType layerMoveEase;
    public Layer Gameplay, Sequences, Decoration;

    [HideInInspector] public bool isEnabled;
    bool alreadyLinked = false;
    float time = 0;

    internal void GameSetup()
    {
        ToggleSceneLayer(Sequences, false);
        ToggleSceneLayer(Decoration, true);
        ToggleSceneLayer(Gameplay, true);
        Setup();

        if (alreadyLinked) return;
        alreadyLinked = true;
        seqLoose.stopped += onLooseSeqEnded;
        seqWin.stopped += onWinSeqEnded;

    }
    private void onWinSeqEnded(PlayableDirector obj)
    {
        DirectorUtils.CompleteStop(obj);

        GameManager.instance.ToggleCurtain(false, () => {
            AllDown(() => { GameManager.instance.NextMiniGame(); });
        });

    }
    private void onLooseSeqEnded(PlayableDirector obj)
    {
        DirectorUtils.CompleteStop(obj);
        bool canReload = GameManager.instance.VerifyTries();
        if (canReload) GameManager.instance.ReloadMiniGame(); else GameManager.instance.EndSession();
    }

    internal void GameBegin()
    {
        isEnabled = true;
        Begin();
    }
    internal void GameOver()
    {
        isEnabled=false;
        Over();
        // Setup seq
        ShowSeqLayer(() =>
        {
            seqLoose.Play();
        });
    }
    internal void GameWin()
    {
        isEnabled = false;
        Win();
        // Setup seq
        ShowSeqLayer(() =>
        {
            seqWin.Play();
        });

    }
    internal void GameTick()
    {
        if (!isEnabled) return;
        if (time > 0) time = Mathf.Max(0, time - Time.deltaTime); else TimerEnded();
        Tick();
    }

    #region Overrides
    internal virtual void Setup(){ }
    internal virtual void Begin() { }
    internal virtual void Over() { }
    internal virtual void Win() { }
    internal virtual void Tick() { }
    internal virtual void TimerEnded()
    {
        GameOver();
    }
    #endregion
    #region Utils
    internal void SetTimer(float value)
    {
        time = Mathf.Clamp(time + value, 0, 30);
    }
    internal void ShowSeqLayer(Action callback, float delayTime = 0.5f)
    {
        GameManager.instance.ToggleCurtain(false, () =>
        {
            ToggleSceneLayer(Gameplay, false);
            ToggleSceneLayer(Sequences, true, () =>
            {
                GameManager.instance.ToggleCurtain(true, callback);
            }, delayTime);
        },0);
    }
    internal void ToggleSceneLayer(Layer layer, bool isVisible, Action callback = null, float callDelay = 0f)
    {
        layer.layerTR.LeanMoveY(isVisible ? layer.onSceneY : layer.offSceneY, layerMoveTime).setEase(layerMoveEase).setOnComplete(() =>
        {
            LeanTween.delayedCall(callDelay, callback);
        });
    }
    internal void AllDown(Action callback)
    {

        ToggleSceneLayer(Gameplay, false);
        ToggleSceneLayer(Sequences, false);
        ToggleSceneLayer(Decoration, false, callback, 1);

    }
    #endregion
}

[System.Serializable]
public struct Layer
{
    public Transform layerTR;
    public float onSceneY, offSceneY;
}
