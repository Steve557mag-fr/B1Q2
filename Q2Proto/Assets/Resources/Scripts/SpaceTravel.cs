using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;

public class SpaceTravel : MiniGame
{
    public int goodPath;

    public Animator animatorCar;

    public SpriteRenderer[] pathHints;
    public SpriteRenderer userSelection;

    public Transform timer;

    public KeyCode up, middle, down;

    int pathOK = 0;
    float timeleft = 30;
    bool canChoose = false;

    public override void StartGame(GameObject area)
    {
        base.StartGame(area);
        timeleft = 30;
        pathOK = 0;
        NextPathsChoice();

    }

    public override void UpdateGame()
    {
        if (softLockMiniGame) return;
        if (timeleft <= 0) LooseGame(looseSequence);
        else timeleft = Mathf.Clamp(timeleft - Time.deltaTime, 0, 30);

        var t = timeleft / 30;
        timer.localScale = new Vector3(t, 1, 1);
        timer.localPosition = new Vector3(-0.5f + (t/ 2), 0, 0);

        if (Input.GetKeyDown(up)) TakePath("Up");
        else if (Input.GetKeyDown(middle)) TakePath("Middle");
        else if (Input.GetKeyDown(down)) TakePath("Down");

    }
    

    public void NextPathsChoice()
    {
        if (pathOK == 3) {
            canChoose = false;
            WinGame();
            return;
        }
        goodPath = Random.Range(0,pathHints.Length);
        ShowPaths();
        canChoose = true;
    }

    void TakePath(string carTrigger)
    {
        if (!canChoose) return;
        canChoose = false;
        animatorCar.SetTrigger(carTrigger);
    }

    public void VerifyPath(int index)
    {
        if (index == goodPath)
        {
            pathOK++;
            NextPathsChoice();
        }
        else
        {
            print("looser");
            LooseGame(looseSequence);
        }
        
    }

    void ShowPaths()
    {
        for(int i = 0; i < pathHints.Length; i++)
        {
            pathHints[i].color = (i == goodPath) ? Color.green : Color.red;
        }
    }

}
