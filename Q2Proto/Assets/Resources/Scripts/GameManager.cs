using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{

    public Color[] colors;
    public Cloth[] cloths;
    public Minigame[] minigames;

    public Menu menu;
    public Evil currentEvil;
    public string   nameClap;
    public Animator animatorClap;
    public PlayableDirector directorBravo;

    int currentMinigame = 0;
    int lifeLeft = 3;

    public void StartSession()
    {
        //0. Reset all minigames
        foreach(Minigame mg in minigames) {
            mg.GameReset();
        }

        //1. Generate evil profile
        currentEvil = new()
        {
            cloth = cloths[Random.Range(0, cloths.Length)],
            tint = colors[Random.Range(0, colors.Length)]
        };

        //2. Initialize variables
        currentMinigame = 0;
        lifeLeft = 3;

        //3. Play the clap animation
        PlayClap();

    }
    
    public void EndSession()
    {

    }

    public void WinSession()
    {

    }

    public void VerifySession()
    {
        lifeLeft--;
        if (lifeLeft <= 0) EndSession();
        else PlayClap();
    }

    public void NextMG()
    {
        currentMinigame += 1;
        if (currentMinigame < minigames.Length) PlayClap();
        else WinSession();
    }

    public void SetupGM() { GetCurrentMG().GameSetup(); }
    public void PlayClap() { animatorClap.Play(nameClap); }
    internal Minigame GetCurrentMG() { return minigames[currentMinigame]; }
    public static GameManager Get() { return FindAnyObjectByType<GameManager>(); }
    public void Update() { if (GetCurrentMG().isEnabled) GetCurrentMG().GameTick(); }

}

public struct Evil
{
    public Cloth cloth;
    public Color tint;
}
