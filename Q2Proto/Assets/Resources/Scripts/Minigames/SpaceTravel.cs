using System;
using System.IO.IsolatedStorage;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpaceTravel : MiniGame
{
    [Header("Game Settings")]
    public Animator vessel;
    public Piece[] pieces;
    public SpriteRenderer[] slots, pathSlots;

    public float[] timerByAnswer;

    public Transform gauge;
    public TextMeshPro textLeft;

    public Sprite goodPathSprite, badPathSprite;

    public Piece[] currentPieces;
    public int currentGoodPathID;

    float currentTimeMax = 30;
    int answersCount = 0;
    bool lockInput = true;

    internal override void Setup()
    {
        answersCount = 0;
        vessel.Play("Reset");
        Player.instance.SetVisible(false);
        GetNewPath();
        
    }

    internal override void Begin()
    {
        base.Begin();
        lockInput = false;
    }

    internal override void Tick()
    {
        if (lockInput) return;
        if (Input.GetKeyDown(Player.instance.keys[0])) PlayVessel(currentPieces[0].pathID);
        if (Input.GetKeyDown(Player.instance.keys[1])) PlayVessel(currentPieces[1].pathID);
        if (Input.GetKeyDown(Player.instance.keys[2])) PlayVessel(currentPieces[2].pathID);

        gauge.transform.localScale = new(Mathf.Lerp(0, 7.63f, timeLeft/currentTimeMax ), 0.61f, 1);

    }

    public void PlayVessel(int pathID)
    {
        lockInput = true;
        vessel.Play("Take_" + pathID);
    }
    public void VerifyPath(int pathID)
    {
        if (pathID != currentGoodPathID) { GameOver(); return; }
        vessel.Play("Reset");

        answersCount++;
        textLeft.text = $"{answersCount}/3";

        if (answersCount < 3) GetNewPath();
        else GameWin();
    }

    public void ShowPieces()
    {
        for(int i = 0; i < currentPieces.Length; i++)
        {
            slots[i].sprite = currentPieces[i].sprite;
        }

        for(int i = 0; i < currentPieces.Length; i++)
        {
            pathSlots[i].sprite = i == currentGoodPathID ? goodPathSprite : badPathSprite;
        }


        textLeft.text = $"{answersCount}/3";

    }
    public void GetNewPath()
    {

        int ITERATION = 20;
        currentGoodPathID = Random.Range(0, 2);
        currentPieces = new Piece[3];
        int picked = 0;

        currentTimeMax = timerByAnswer[answersCount];
        SetTimer(currentTimeMax);

        //Pick a good piece
        Piece[] goodP = Array.FindAll(pieces, p => p.pathID == currentGoodPathID);
        currentPieces[0] = goodP[Random.Range(0,goodP.Length)];

        //Pick bad pieces
        for(int i = 0; i < ITERATION; i++)
        {
            if (picked >= 2) break;

            int idPiece = Random.Range(0, pieces.Length);
            Piece p = pieces[idPiece];
            
            if (p.pathID != currentGoodPathID)
            {
                currentPieces[1+picked] = p;
                picked++;
            }
        }

        for(int i = 0; i < ITERATION; i++)
        {
            int a = Random.Range(0, 3); int b = Random.Range(0, 3);
            var c = currentPieces[a];
            currentPieces[a] = currentPieces[b];
            currentPieces[b] = c;
        }

        //Show pieces
        ShowPieces();

        //Unlock inputs
        lockInput = false;
    }

}

[System.Serializable]
public struct Piece
{
    public Sprite sprite;
    [Range(0,2)] public int pathID;
}
