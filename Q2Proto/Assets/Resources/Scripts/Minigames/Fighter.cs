using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fighter : Minigame
{
    [SerializeField] float spawnRate, attackRange, QTETimeMax, objective;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform a, b;
    [SerializeField] TextMeshPro textPercent;

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

        textPercent.text = $"{Mathf.RoundToInt((1 - (timeLeft / timeMax)) * 100)} %";

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
