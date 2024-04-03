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
    public Outfit[] outfits;
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

    bool softLockSession = false;

    public void StartSession()
    {
        if (softLockSession) return;
        softLockSession = true;
        // Reset count of tries
        tries = 3;

        // Take a random evil
        currentEvil = new Evil()
        {
            outfitID = Random.Range(0, outfits.Length),
            colorID = Random.Range(0, colors.Length)
        };

        // Hide GUI Elements
        HideGUI(() =>
        {
            currentMiniGame = -1;
            NextMiniGame();
        });
        
    }
    public void EndSession()
    {
        softLockSession = false;
        UpdateSpots(true);
        tries = 0;
        ShowGUI();
    }
    public void WinSession()
    {
        print("BRAWOO ");
    }


    

    public void NextMiniGame()
    {
        Debug.Log("[GM]: Next MG");
        isStarted = false;
        currentMiniGame++;
        if (currentMiniGame >= miniGames.Length)
        {
            WinSession();
            return;
        }

        titleClap.text = miniGames[currentMiniGame].tileText;
        descrClap.text = miniGames[currentMiniGame].descriptionText;
        controller.Play(clapNameState);
    }
    public void EnableMiniGame()
    {
        Debug.Log("[GM]: Enable MG");

        currentMiniGame = Mathf.Clamp(currentMiniGame, 0, miniGames.Length - 1);

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
    public void HideGUI(System.Action afterStart)
    {
        LeanTween.moveLocalY(TitleSign, offScreenYTitle, transitionTime).setEase(easeType);
        LeanTween.moveLocalY(GUIFrame, offScreenYGUI, transitionTime).setEase(easeType).setOnComplete(() =>
        {
            LeanTween.delayedCall(delayStartSession, () => {
                afterStart();
            });
        });
    }
    public void UpdateSpots(bool forceOn = false)
    {
        for (var i = 0; i < spots.Length; i++)
        {
            spots[i].gameObject.SetActive(forceOn || i < tries);
        }
    }


    public void GameOver()
    {
        // Destroy the GameArea
        if (gameArea != null) Destroy(gameArea);

        // Update the number of tries + spots
        tries = Mathf.Clamp(tries - 1, 0, 3);
        UpdateSpots();

        // Check if is the end ? 
        if (tries <= 0)
        {
            // End Session
            print("ending..");
            EndSession();
            return;
        }

        // Reload the current MiniGame
        print("reloading..");
        currentMiniGame--;
        NextMiniGame();


    }
    

    public static GameObject GetEnemy(bool isEvil, int countOutfit = 1)
    {
        // get the evil base
        GameManager instance = GetInstance();
        Evil evil = instance.currentEvil;
        GameObject go = Instantiate(Resources.Load<GameObject>($"Prefabs/Enemy_{Random.Range(0,2)}"));
        
        // set outfits
        for(int i = 0; i < countOutfit; i++)
        {
            Outfit outfit = instance.outfits[isEvil ? evil.outfitID : Random.Range(0, instance.outfits.Length)];
            GameObject gOutfit = Instantiate(outfit.asset, go.transform);
            gOutfit.transform.localPosition = outfit.position;

            if (gOutfit.transform.Find("Tintable"))
            {
                gOutfit.transform.Find("Tintable").GetComponent<SpriteRenderer>().color = isEvil ? instance.colors[evil.colorID] : Color.white;
            }
        }
        return go;
    }
    public static GameManager GetInstance()
    {
        return FindAnyObjectByType<GameManager>();
    }
    

    void Update()
    {
        if (isStarted) miniGames[Mathf.Clamp(currentMiniGame, 0, miniGames.Length-1)].UpdateGame();
    }
    
}

public struct Evil
{
    public int outfitID;
    public int colorID;
}
