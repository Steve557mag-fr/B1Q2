using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FindEnemies : MiniGame
{
    [SerializeField] KeyCode approve, denonce; 
    [SerializeField] Player player;
    [SerializeField] float chanceForEvil = 25;
    [SerializeField] Transform[] points;
    Seat[] seats;

    int index = 1;
    float globalTimer = 30;

    public override void StartGame(GameObject area)
    {
        base.StartGame(area);

        globalTimer = 30;
        int countEnemies = points.Length;
        seats = new Seat[countEnemies];
        for(int i = 0; i < countEnemies; i++)
        {
            seats[i].isEvil = Random.Range(0, 100) >= chanceForEvil;
            seats[i].cutout = GameManager.GetEnemy(seats[i].isEvil, Random.Range(1,3));
            seats[i].cutout.transform.position = points[i].position;
            seats[i].cutout.transform.parent = area.transform;
            seats[i].cutout.GetComponent<CutoutBehaviour>().destination = seats[i].cutout.transform.position.x;
            seats[i].cutout.GetComponent<CutoutBehaviour>().horizontalWalkDistance = 0;
        }
        player.isFreeze = true;
    }

    public override void UpdateGame()
    {
        base.UpdateGame();

        if (Input.GetKeyDown(player.left))
        {
            index = Mathf.Clamp(index - 1, 0, points.Length-1);
            player.SetDestination(points[index].position.x);
        }

        if (Input.GetKeyDown(player.right))
        {
            index = Mathf.Clamp(index + 1, 0, points.Length-1);
            player.SetDestination(points[index].position.x);
        }

        if (Input.GetKeyDown(approve)) Approve();
        if (Input.GetKeyDown(denonce)) Denonce();

        if (globalTimer > 0) globalTimer = MathF.Max(0, globalTimer - Time.deltaTime);
        else LooseGame(looseSequence);

        int cSigned = 0;
        for(int i = 0; i < seats.Length; i++)
        {
            cSigned += seats[i].signed ? 1 : 0;
        }

        if (cSigned == seats.Length) WinGame();

    }

    public void Approve()
    {
        if (seats[index].signed) return;
        if (seats[index].isEvil) { print("CAUSE BY APPROVED"); LooseGame(looseSequence); return; }
        // play sign document sequence (haha)
        seats[index].signed = true;
        Debug.Log($"[MG_{tileText}]: APPROVED!");
        Destroy(seats[index].cutout);
    }

    public void Denonce()
    {
        if (seats[index].signed) return;
        if (!seats[index].isEvil) { print("CAUSE BY DENONCED"); LooseGame(looseSequence); return; }
        // play remove evil sequence (haha)
        seats[index].signed = true;
        Debug.Log($"[MG_{tileText}]: DENONCED!");
        Destroy(seats[index].cutout);
    }

}

public struct Seat
{
    public bool signed;
    public bool isEvil;
    public GameObject cutout;
}