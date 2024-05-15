using System;
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
    public ObjectsAnimatorController Gameplay, Sequences, Decoration;

    [HideInInspector] public bool isEnabled;
    bool alreadyLinked = false, timerEnabled = true;
    float time = 0;
    int ind = 0;

    internal void GameSetup()
    {
        Gameplay.Play(true);
        Decoration.Play(true, GameBegin );
        Sequences.Play(false);
        Setup();

        if (alreadyLinked) return;
        alreadyLinked = true;
        seqLoose.stopped += onLooseSeqEnded;
        seqWin.stopped += onWinSeqEnded;
    }
    private void onWinSeqEnded(PlayableDirector obj)
    {
        DirectorUtils.CompleteStop(obj);
        GameManager.instance.ToggleCurtain(false, () => { AllDown(); });
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
        timerEnabled = true;
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
        if(timerEnabled) if (time > 0 ) time = Mathf.Max(0, time - Time.deltaTime); else TimerEnded();
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
    internal void SetActiveTimer(bool isActive) { timerEnabled = isActive; }
    internal void SetTimer(float value)
    {
        time = Mathf.Clamp(time + value, 0, 30);
    }
    internal void ShowSeqLayer(Action callback, float delayTime = 0.5f)
    {
        LeanTween.delayedCall(delayTime, ()=>{
            Gameplay.Play(false);
            Sequences.Play(true, callback);
        });
    }
    internal void AllDown()
    {
        ind = 0;
        Gameplay.Play(false,   onFinishDown);
        Decoration.Play(false, onFinishDown);
        Sequences.Play(false,  onFinishDown);
    }
    void onFinishDown()
    {
        ind++;
        if(ind > 3) GameManager.instance.NextMiniGame();
    }
    internal float timeLeft
    {
        get { return time; }
        set { SetTimer(value); }
    }
    #endregion
}

