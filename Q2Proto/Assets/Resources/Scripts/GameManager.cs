using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Header("Mini Games")]
    public int tries = 3;
    public bool isStarted;
    public int currentMiniGame;
    public MiniGame[] miniGames;
    public GameObject[] spots;
    GameObject gameArea;

    [Header("Enemies")]
    public int hatID;
    public int signID, colorID;
    public Sprite[] hats, signs;
    public Color[] colors;

    [Header("Menu Transition")]
    public float transitionTime = 0.1f;
    public float delayStartSession = 0.5f;
    public GameObject GUIFrame, TitleSign;
    public LeanTweenType easeType = LeanTweenType.easeOutCubic;
    public float offScreenYTitle = -30f, offScreenYGUI = -200f;

    [Header("Clap Settings")]
    public Animator controller;
    public string clapNameState = "clap";
    public TextMeshPro titleClap, descrClap;


    public void StartSession()
    {
        LeanTween.moveLocalY(TitleSign, offScreenYTitle, transitionTime).setEase(easeType);
        LeanTween.moveLocalY(GUIFrame, offScreenYGUI, transitionTime).setEase(easeType).setOnComplete(() =>
        {
            LeanTween.delayedCall(delayStartSession, ()=>{
                currentMiniGame = -1;
                NextMiniGame();
            });
        });
    }

    public void NextMiniGame()
    {
        currentMiniGame++;
        titleClap.text = miniGames[currentMiniGame].tileText;
        descrClap.text = miniGames[currentMiniGame].descriptionText;
        controller.Play(clapNameState);
    }
    public void EnableMiniGame()
    {
        if (gameArea != null) Destroy(gameArea);
        gameArea = new GameObject("GameArea");
        miniGames[currentMiniGame].StartGame(gameArea);
        isStarted = true;
    }

    void Update()
    {

        if (isStarted)
        {
            miniGames[Mathf.Min(0, currentMiniGame)].UpdateGame();
        }

    }

    public static void GameOver()
    {
        var go = FindAnyObjectByType<GameManager>();
        if (go.gameArea != null) Destroy(go.gameArea);

        go.tries--;
        for (var i = 0; i < go.spots.Length; i++)
        {
            go.spots[i].gameObject.SetActive(i < go.tries);
        }


        go.currentMiniGame--;
        go.NextMiniGame();
    }

}
