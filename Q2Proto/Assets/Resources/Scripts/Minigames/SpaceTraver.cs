using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpaceTraver : Minigame
{

    [Header("MG Settings")]
    [SerializeField] List<Piece> pieces;
    [SerializeField] Animator animatorCar, animatorSky;
    [SerializeField] float[] timingByAnswers;
    [SerializeField] GameObject currentTimeBar;

    [SerializeField] SpriteRenderer[] pieceSlots;
    [SerializeField] SpriteRenderer[] indicatorPaths;
    [SerializeField] Sprite badIndicator;
    [SerializeField] TextMeshPro textSignState;

    [SerializeField] float scaleTimeBar;

    int currAnswer  = 0;
    int MAX_ANSW = 3;

    int goodPath;
    Piece[] currentPaths;

    string[] ANIMATOR_CLIP_NAMES = new string[] { "DOWN" , "MIDDLE", "UP" };

    internal override void Begin()
    {
        ToggleAnimators(true);
        GenerateNewCase();
    }

    internal override void Tick()
    {

        if (isTimerLocked) return;

        float currBarValue = scaleTimeBar * (timeLeft / timingByAnswers[currAnswer]);
        currentTimeBar.transform.localScale = new Vector3(currBarValue , currentTimeBar.transform.localScale.y, currentTimeBar.transform.localScale.z);

        if (Input.GetKeyDown(Player.Get().Action1)) PlayPath(0);
        else if (Input.GetKeyDown(Player.Get().Action2)) PlayPath(1);
        else if (Input.GetKeyDown(Player.Get().Action3)) PlayPath(2);

    }

    internal override void Over()
    {
        ToggleAnimators(false);
    }
    internal override void Win()
    {
        ToggleAnimators(false);
    }

    #region MiniGameMethods

    void ToggleAnimators(bool enabled)
    {
        animatorSky.enabled = enabled;
        animatorCar.enabled = enabled;
    }

    void GenerateNewCase()
    {

        timeLeft = timingByAnswers[currAnswer];

        goodPath = Random.Range(0, 3);

        var possibilities = pieces.FindAll((Piece p) => { return p.goodIndex == goodPath; });
        Piece goodOne = possibilities[Random.Range(0, possibilities.Count)];


        currentPaths = new Piece[] { goodOne, pieces[Random.Range(0, pieces.Count)], pieces[Random.Range(0, pieces.Count)] }; // change this shit.

        UpdateGraphic();

        isTimerLocked = false;

    }

    void UpdateGraphic()
    {

        for(int i = 0; i < pieceSlots.Length; i++)
        {
            pieceSlots[i].sprite = pieces[i].sprite;
            indicatorPaths[i].sprite = (i == goodPath) ? null : badIndicator;
        }

        textSignState.text = $"{currAnswer}/3";

    }

    void PlayPath(int index)
    {
        print("AAA");
        isTimerLocked = true;
        animatorCar.Play(ANIMATOR_CLIP_NAMES[currentPaths[index].goodIndex]);
    }

    public void VerifyAnswer(int path)
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

    #endregion

}

[Serializable]
public struct Piece
{
    public Sprite sprite;
    public int goodIndex;
} 