using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpaceTraver : Minigame
{

    [Header("MG Settings")]
    [SerializeField] List<Piece> pieces;
    [SerializeField] Animator animatorCar, animatorSky;
    [SerializeField] float[] timingByAnswers;
    int currAnswer  = 0;
    int MAX_ANSW = 3;

    int goodPath;
    Piece[] currentPaths;

    string[] ANIMATOR_CLIP_NAMES = new string[] { "DOWN" , "MIDDLE", "UP" };

    internal override void Begin()
    {

        animatorSky.enabled = true;
        animatorCar.enabled = true;

        GenerateNewCase();

    }

    internal override void Tick()
    {

        if (isTimerLocked) return;

        if (Input.GetKeyDown(Player.Get().Action1)) PlayPath(0);
        else if (Input.GetKeyDown(Player.Get().Action2)) PlayPath(1);
        else if (Input.GetKeyDown(Player.Get().Action3)) PlayPath(2);

    }

    void GenerateNewCase() {

        timeLeft = timingByAnswers[currAnswer];

        goodPath = Random.Range(0, 3);

        var possibilities = pieces.FindAll((Piece p) => { return p.goodIndex == goodPath; });
        Piece goodOne = possibilities[ Random.Range(0, possibilities.Count)];


        currentPaths = new Piece[] { goodOne, pieces[Random.Range(0, pieces.Count)], pieces[Random.Range(0, pieces.Count)] }; // change this shit.

        UpdateGraphic();

        isTimerLocked = false;

    }

    void UpdateGraphic()
    {

    }

    void PlayPath(int index)
    {
        isTimerLocked = true;
        animatorCar.Play(ANIMATOR_CLIP_NAMES[ currentPaths[index].goodIndex ]);
    }

    void VerifyAnswer(int path)
    {

        if (path == goodPath)
        {
            //win state
            currAnswer++;

            if (currAnswer >= MAX_ANSW) GameWin();
            else GenerateNewCase();
        }
        else GameOver();

    }

}

[Serializable]
public struct Piece
{
    public Sprite sprite;
    public int goodIndex;
} 