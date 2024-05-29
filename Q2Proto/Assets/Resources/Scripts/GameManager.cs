using System;
using TMPro;
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

    public TextMeshPro txtTitle, txtDescription, txtLife;

    int currentMinigame = 0;
    int lifeLeft = 3;

    bool canStart = true;
    bool sessionEnabled = false;

    public void StartSession()
    {
        if (!canStart) return;
        canStart = false;

        //0. Reset all minigames
        foreach (Minigame mg in minigames) {
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
        sessionEnabled = true;
        PlayClap();
    }
    
    public void EndSession()
    {
        SetCurtain(false, () =>
        {
            Menu.instance.ToggleMenuPanel(true,null);
            canStart = true;
        });
    }

    bool alreadyBravo;
    public void WinSession()
    {
        sessionEnabled = false;

        SetCurtain(true, () =>
        {
            directorBravo.Play();
        });

        if (alreadyBravo) return;
        alreadyBravo = true;
        directorBravo.stopped += (PlayableDirector d) =>
        {
            EndSession();
        };  
    }

    public void VerifySession()
    {
        lifeLeft--;
        if (lifeLeft <= 0) EndSession();
        else PlayClap();
    }

    public void NextMG()
    {
        sessionEnabled = false;
        print("lock session");
        SetCurtain(false, () => {
            GetCurrentMG().GameReset();
            currentMinigame += 1;
            if (currentMinigame < minigames.Length) {
                print("unlock session");
                sessionEnabled = true;
                PlayClap();
            } else
            {
                print("still lock session");
                WinSession();
            };
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
    public void PlayClap() {

        txtTitle.text = GetCurrentMG().title;
        txtDescription.text = GetCurrentMG().description;
        txtLife.text = $"{lifeLeft} LIVES LEFT";
        animatorClap.Play(nameClap);
    }
    internal Minigame GetCurrentMG() { return minigames[Mathf.Clamp(currentMinigame,0,minigames.Length-1)]; }
    public static GameManager Get() { return FindAnyObjectByType<GameManager>(); }
    public void Update() {
        if (GetCurrentMG().isEnabled && sessionEnabled)
        {
            GetCurrentMG().GameTick();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus)) GetCurrentMG().GameWin();
        if (Input.GetKeyDown(KeyCode.KeypadMinus)) GetCurrentMG().GameOver();

    }

}

[System.Serializable]
public struct Evil
{
    public Cloth cloth;
    public Color tint;
}
