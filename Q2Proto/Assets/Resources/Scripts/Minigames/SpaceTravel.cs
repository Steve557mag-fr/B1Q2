using System;
using System.IO.IsolatedStorage;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpaceTravel : MiniGame
{
    [Header("Game Settings")]
    public Animator vessel;
    public Piece[] pieces;
    public SpriteRenderer[] slots, pathSlots;

    public Sprite goodPathSprite, badPathSprite;

    public Piece[] currentPieces;
    public int currentGoodPathID;

    int answersCount = 0;
    bool lockInput = true;

    internal override void Setup()
    {
        vessel.Play("Reset");
        GetNewPath();
    }

    internal override void Begin()
    {
        answersCount = 0;
        SetTimer(30);
    }

    internal override void Tick()
    {
        if (lockInput) return;
        if (Input.GetKeyDown(Player.instance.keys[0])) PlayVessel(currentPieces[0].pathID);
        if (Input.GetKeyDown(Player.instance.keys[1])) PlayVessel(currentPieces[1].pathID);
        if (Input.GetKeyDown(Player.instance.keys[2])) PlayVessel(currentPieces[2].pathID);
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
    }
    public void GetNewPath()
    {

        int ITERATION = 20;
        currentGoodPathID = Random.Range(0, 2);
        currentPieces = new Piece[3];
        int picked = 0;

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
