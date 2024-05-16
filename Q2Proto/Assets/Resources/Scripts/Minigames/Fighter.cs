using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Minigame
{
    [SerializeField] float spawnRate, attackRange, QTETimeMax, objective;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform a, b;

    List<CutoutBehaviour> cutouts;
    CutoutBehaviour target;

    float timeSpawn;

    bool isBusy;

    internal override void Setup()
    {
        cutouts = new List<CutoutBehaviour>();
    }

    internal override void Tick()
    {



        foreach(CutoutBehaviour cutout in cutouts)
        {
            if(Vector2.Distance(cutout.transform.position, cutout.transform.position) < attackRange)
            {
                target = cutout;
                EngageQTE();
            }
        }

    }

    void TickQTE()
    {

    }

    void EngageQTE()
    {

    }

    void LeaveQTE()
    {

    }

}
