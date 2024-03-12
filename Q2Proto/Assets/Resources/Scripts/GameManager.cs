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

    [Header("Enemy Target")]
    public Evil currentEvil;
    public Sprite[] hats;
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

        // Take a random evil
        currentEvil = new Evil()
        {
            hatID = Random.Range(0, hats.Length),
            colorID = Random.Range(0, colors.Length)
        };

        // Hide GUI Elements
        HideGUI();
        
    }
    public void EndSession()
    {
        tries = 99; // just for the spot update
        UpdateSpots();
        ShowGUI();
    }
    public void WinSession()
    {
        print("BRAWOO ");
    }


    public void NextMiniGame()
    {
        currentMiniGame++;

        if (currentMiniGame >= miniGames.Length) WinSession();

        titleClap.text = miniGames[currentMiniGame].tileText;
        descrClap.text = miniGames[currentMiniGame].descriptionText;
        controller.Play(clapNameState);
    }
    public void EnableMiniGame()
    {
        if (gameArea != null) Destroy(gameArea);
        gameArea = new GameObject("GameArea");
        gameArea.transform.parent = miniGames[currentMiniGame].decorationLayer.transform;
        miniGames[currentMiniGame].StartGame(gameArea);
        isStarted = true;
    }
    

    public void ShowGUI()
    {
        LeanTween.moveLocalY(TitleSign, 3.17f, transitionTime).setEase(easeType);
        LeanTween.moveLocalY(GUIFrame, -600f, transitionTime).setEase(easeType);
    }
    public void HideGUI()
    {
        LeanTween.moveLocalY(TitleSign, offScreenYTitle, transitionTime).setEase(easeType);
        LeanTween.moveLocalY(GUIFrame, offScreenYGUI, transitionTime).setEase(easeType).setOnComplete(() =>
        {
            LeanTween.delayedCall(delayStartSession, () => {
                currentMiniGame = -1;
                NextMiniGame();
            });
        });
    }
    public void UpdateSpots()
    {
        for (var i = 0; i < spots.Length; i++)
        {
            spots[i].gameObject.SetActive(i < tries);
        }
    }


    public void GameOver()
    {
        // Get the GameManager
        if (gameArea != null) Destroy(gameArea);

        // Update the number of tries + spots
        tries--;
        UpdateSpots();

        // Check if is the end ? 
        if (tries == 0)
        {
            // End Session
            EndSession();
        }
        else
        {
            // Reload the current MiniGame
            currentMiniGame--;
            NextMiniGame();
        }

    }
    public static Evil GetEvil()
    {
        return FindAnyObjectByType<GameManager>().currentEvil;
    }
    public static GameManager GetInstance()
    {
        return FindAnyObjectByType<GameManager>();
    }


    void Update()
    {
        if (isStarted) miniGames[Mathf.Min(0, currentMiniGame)].UpdateGame();
    }
    
}

public struct Evil
{
    public int hatID;
    public int colorID;
}
