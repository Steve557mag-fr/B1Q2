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
    public float transitionTime = 0.5f;
    public LeanTweenType transitionEase;
    public Layer Gameplay, Sequences, Decoration;

    [HideInInspector] public bool isEnabled;
    bool alreadyLinked = false;

    float time = 0;

    public void Initialize()
    {
        SwitchState(Decoration, true);
        SwitchState(Gameplay, true, () =>
        {
            onBegin.Invoke();
            Begin();
            isEnabled = true;
        });

        if (alreadyLinked) return;
        alreadyLinked = true;

        seqLoose.stopped += OnLooseSeqEnded;
        seqWin.stopped += OnWinSeqEnded;
    }

    public void UpdateTimer()
    {
        if (time <= 0) { GameOver(); }
        else time -= Time.deltaTime;

        Tick();

    }

    public void SetTimer(float t)
    {
        time = Mathf.Clamp(t,0,30);
    }

    void SwitchState(Layer layer, bool onStage, System.Action callback = null)
    {
        layer.layerTR.LeanMoveY(onStage ? layer.onSceneY : layer.offSceneY, transitionTime).setEase(transitionEase);
        callback?.Invoke();
    }

    public virtual void Begin() { }
    public virtual void Tick() { }
    public virtual void Loose() { }

    public void GameOver()
    {
        isEnabled = false;
        Loose();
        onEnded.Invoke();

        // Setup all layers (down GAMEPLAY, up SEQUENCE)
        SwitchState(Gameplay, false);
        SwitchState(Sequences, true, () =>
        {
            // Play loose sequence
            seqLoose.Play();

        });
    }    
    public void Win()
    {
        isEnabled = false;
        onEnded.Invoke();

        // Setup all layers (down GAMEPLAY, up SEQUENCE)
        SwitchState(Gameplay, false);
        SwitchState(Sequences, true, () =>
        {
            // Play win sequence
            seqWin.Play();

        });
    }

    public virtual void OnLooseSeqEnded(PlayableDirector obj)
    {
        DirectorUtils.CompleteStop(obj);

        bool canReload = GameManager.instance.VerifyTries();
        if(canReload)
        {
            GameManager.instance.RestartCurrentMG();
        }
    }
    public virtual void OnWinSeqEnded(PlayableDirector obj)
    {
        DirectorUtils.CompleteStop(obj);
        GameManager.instance.NextMiniGame();
    }
}

public struct Layer
{
    public Transform layerTR;
    public float onSceneY, offSceneY;
}
