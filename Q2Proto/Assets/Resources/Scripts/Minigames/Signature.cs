using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Signature : MiniGame
{
    [Header("Game Settings")]
    public CutoutBehaviour[] cutouts;
    public People[] peoples;
    public int peopleSelected = 0;

    internal override void Setup()
    {
        peoples = new People[cutouts.Length];
        for(int i = 0; i < cutouts.Length; i++)
        {
            peoples[i].cutout = cutouts[i];
            peoples[i].isEvil = Random.Range(0, 100) >= 50f;
            peoples[i].cutout.RandomizeWalkTimer();
        }

        peopleSelected = 0;
        Player.instance.isFreeze = true;
        Player.instance.SetVisible(true);
        ApplyMovement(-peopleSelected);
    }

    internal override void Begin()
    {
        SetTimer(15);
    }

    internal override void Tick()
    {

        if (!Player.instance.CanMove) return;
        
        if (Input.GetKeyDown(KeyCode.A)) ApplyMovement(-1);
        if (Input.GetKeyDown(KeyCode.D)) ApplyMovement(1);

        if (peoples[peopleSelected].isInteracted) return;
        if (Input.GetKeyDown(KeyCode.J)) Approve();
        if (Input.GetKeyDown(KeyCode.K)) Denonce();

    }

    void Approve()
    {
        if (peoples[peopleSelected].isEvil) GameOver();
        else
        {
            peoples[peopleSelected].isInteracted = true;
            //do some code...
        }
    }

    void Denonce()
    {
        if (!peoples[peopleSelected].isEvil) GameOver();
        else
        {
            peoples[peopleSelected].isInteracted = true;
            //do some code...
        }
    }

    internal void ApplyMovement(int i)
    {
        peopleSelected = Mathf.Clamp(peopleSelected + i, 0, peoples.Length-1);
        float newX = peoples[peopleSelected].cutout.transform.position.x;
        Player.instance.SetDestination(newX);
    }


}

public struct People
{
    public bool isInteracted;
    public bool isEvil;
    public CutoutBehaviour cutout;
}