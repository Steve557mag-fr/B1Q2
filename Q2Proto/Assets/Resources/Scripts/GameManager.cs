using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [Header("Clap")]
    public TextMeshPro clapTitleText;
    public TextMeshPro clapTriesText, clapDescriptionText;
    public Animator clapAnimator;
    public string clapStateName;

    [Header("Ending")]
    public PlayableDirector bravoSeq;


    [SerializeField] MiniGame[] miniGames;

    int tries = 3;
    [HideInInspector] public int currentMG = 0;
    bool isStarted = false;

    private void Awake()
    {
        isStarted = false;
        bravoSeq.stopped += onBravoSeqEnded;
    }

    void onBravoSeqEnded(PlayableDirector dir)
    {
        DirectorUtils.CompleteStop(dir);
    }

    public void StartSession()
    {
        if (isStarted) return;
        isStarted = true;
        tries = 3;
        currentMG = 0;

        PlayMiniGame(currentMG);

    }

    public void PlayClap(int iMG)
    {
        clapTriesText.text = $"{tries} tries left";
        clapTitleText.text = miniGames[iMG].title;
        clapDescriptionText.text = miniGames[iMG].description;
        clapAnimator.Play(clapStateName);
    }

    public void PlayEnding()
    {
        bravoSeq.Play();
    }

    public void EndSession()
    {


        isStarted = false;
    }

    public bool VerifyTries()
    {
        tries--;
        if (tries == 0) EndSession();
        return tries > 0;
    }

    public void PlayMiniGame(int index)
    {
        miniGames[currentMG].Initialize();
    }

    public void RestartCurrentMG()
    {
        PlayClap(currentMG);
    }

    public void NextMiniGame()
    {

        currentMG++;
        if(currentMG == miniGames.Length) PlayEnding();
        else PlayClap(currentMG);

    }

    public void Update()
    {
        if (isStarted && miniGames[currentMG].isEnabled) miniGames[currentMG].UpdateTimer();
    }
    
    public static GameManager instance
    {
        get { return FindObjectOfType<GameManager>(); }
    }

}
