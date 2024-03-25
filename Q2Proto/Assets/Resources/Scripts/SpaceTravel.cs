using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceTravel : MiniGame
{
    public int goodPath;

    public SpriteRenderer[] pathHints;
    public SpriteRenderer userSelection;

    public KeyCode up, middle, down;

    int pathOK = 0;

    public override void StartGame(GameObject area)
    {
        base.StartGame(area);

        pathOK = 0;
        NextPathsChoice();

    }

    public override void UpdateGame()
    {
        base.UpdateGame();

        if (Input.GetKeyDown(up)) TakePath(0);
        else if (Input.GetKeyDown(middle)) TakePath(1);
        else if (Input.GetKeyDown(down)) TakePath(2);

    }
    

    public void NextPathsChoice()
    {
        goodPath = Random.Range(0,pathHints.Length);
        ShowPaths();

    }

    void TakePath(int index)
    {
        if(index == goodPath)
        {
            pathOK++;
            NextPathsChoice();
        }
        else
        {
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
