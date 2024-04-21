using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Clap")]
    public TextMeshPro clapTitleText;
    public TextMeshPro clapTriesText, clapDescriptionText;
    public Animator clapAnimator;
    public string clapStateName;

    [Header("Curtain")]
    public Animator curtainAnim;
    public float curtainTime;

    [Header("Ending")]
    public PlayableDirector bravoSeq;


    [SerializeField] MiniGame[] miniGames;

    [Header("Evil Settings")]
    public EvilProfile evilProfile;
    [SerializeField] Color[] evilColors;
    [SerializeField] Outfit[] evilOutfits;

    int tries = 3;
    [HideInInspector] public int currentMGIndex = 0;
    bool isStarted = false;

    private void Awake()
    {
        isStarted = false;
        bravoSeq.stopped += onBravoSeqEnded;
    }

    void onBravoSeqEnded(PlayableDirector dir)
    {
        DirectorUtils.CompleteStop(dir);
        EndSession();
    }

    public void StartSession()
    {
        if (isStarted) return;
        isStarted = true;
        tries = 3;
        currentMGIndex = 0;
        GenerateEvilProfile();
        PlayClap(0);
    }

    public void PlayClap(int iMG)
    {
        
        clapTriesText.text = $"{tries} tries left";
        clapTitleText.text = miniGames[iMG].title;
        clapDescriptionText.text = miniGames[iMG].description;
        clapAnimator.Play(clapStateName);
        GetCurrentMiniGame().GameSetup();
    }

    public void PlayEnding()
    {
        isStarted = false;

        GetCurrentMiniGame().AllDown(() =>
        {
            ToggleCurtain(true);
            bravoSeq.Play();
        });
    }

    public void EndSession()
    {
        isStarted = false;
        ToggleCurtain(false, () =>
        {
            Menu.instance.ToggleMenuPanel(true, () => { });
        });
    }

    public bool VerifyTries()
    {
        tries--;
        if (tries == 0) EndSession();
        return tries > 0;
    }


    public MiniGame GetCurrentMiniGame()
    {
        return miniGames[Math.Clamp(currentMGIndex,0, miniGames.Length-1)];
    }

    public void ToggleCurtain(bool isOpen, Action callback = null, float delayTime = 1)
    {
        curtainAnim.SetBool("isOpen",isOpen);
        LeanTween.delayedCall(curtainTime+delayTime, callback);
    }

    public void Update()
    {
        if (isStarted && miniGames[currentMGIndex].isEnabled) miniGames[currentMGIndex].GameTick();
    }

    internal void ReloadMiniGame()
    {
        ToggleCurtain(false, () => {
            PlayClap(currentMGIndex);
        });
    }

    internal void NextMiniGame()
    {
        currentMGIndex++;
        if (currentMGIndex >= miniGames.Length) PlayEnding(); else PlayClap(currentMGIndex);
    }

    internal void GenerateEvilProfile()
    {
        evilProfile = new EvilProfile()
        {
            color = evilColors[Random.Range(0, evilColors.Length)],
            outfitID = evilOutfits[Random.Range(0, evilOutfits.Length)]
        };
    }

    public static GameManager instance
    {
        get { return FindObjectOfType<GameManager>(); }
    }

}

public struct EvilProfile
{
    public Color color;
    public Outfit outfitID;
}