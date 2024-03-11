using UnityEngine;
using TMPro;

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
        // Reset count of tries
        tries = 3;

        // Hide GUI Elements
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
        if (isStarted) miniGames[Mathf.Min(0, currentMiniGame)].UpdateGame();
    }

    public void ShowGUI()
    {
        LeanTween.moveLocalY(TitleSign, 3.17f, transitionTime).setEase(easeType);
        LeanTween.moveLocalY(GUIFrame, -600f, transitionTime).setEase(easeType);
    }

    public void UpdateSpots()
    {
        for (var i = 0; i < spots.Length; i++)
        {
            spots[i].gameObject.SetActive(i < tries);
        }
    }

    public static void GameOver()
    {
        // Get the GameManager
        var gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager.gameArea != null) Destroy(gameManager.gameArea);

        // Update the number of tries + spots
        gameManager.tries--;
        gameManager.UpdateSpots();

        // Check if is the end ? 
        if (gameManager.tries == 0)
        {

            gameManager.tries = 99; // just for the spot update
            gameManager.UpdateSpots();
            gameManager.ShowGUI();

        }
        else
        {
            // Reload the current MiniGame
            gameManager.currentMiniGame--;
            gameManager.NextMiniGame();
        }

    }

}
