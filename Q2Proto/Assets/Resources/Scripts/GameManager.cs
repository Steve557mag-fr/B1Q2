using System;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public Color[] colors;
    public Cloth[] cloths;
    public Minigame[] minigames;

    public Menu menu;
    public Evil currentEvil;
    public string   nameClap;
    
    public PlayableDirector directorBravo;
    public Animator animatorClap, animatorCurtain;
    public float timeCurtain;

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
        SetCurtain(false, () =>
        {
            Menu.instance.ToggleMenuPanel(true,null);
        });
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

        SetCurtain(false, () => {
            GetCurrentMG().GameReset();
            currentMinigame += 1;
            if (currentMinigame < minigames.Length) {
                PlayClap();
            } else WinSession();
        });

    }

    public void SetCurtain(bool isOpen, Action callback) {
        string animName = isOpen ? "Open" : "Close";
        animatorCurtain.SetTrigger (animName);
        LeanTween.delayedCall(timeCurtain, () =>
        {
            animatorCurtain.ResetTrigger(animName);
            callback();
        });
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
