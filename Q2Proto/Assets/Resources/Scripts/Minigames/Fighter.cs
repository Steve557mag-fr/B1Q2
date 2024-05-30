using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Fighter : Minigame
{
    [SerializeField] float spawnRate, attackRange, QTETimeMax, objective, minFocus, maxFocus;
    [SerializeField] GameObject carScene;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] RigidMovement playerScene;
    [SerializeField] Transform a, b;
    [SerializeField] TextMeshPro textPercent;
    [SerializeField] float timeCameraQte;
    [SerializeField] LeanTweenType easeCameraQte;
    [SerializeField] ObjectsAminator[] letterControllers;
    [SerializeField] TextMeshPro[] letterTexts;
    [SerializeField] Transform[] points;
    [SerializeField] GameObject lettersContainer;
    List<CutoutBehaviour> cutouts;
    CutoutBehaviour target;

    float timeSpawn, timeQTE, maxQTETime = 5;

    bool isBusy;
    bool QTETicking;

    int currKey;
    KeyCode[] qte , keys;

    internal override void Setup()
    {
        carScene.SetActive(true);
        cutouts = new List<CutoutBehaviour>();
        qte = new KeyCode[3];
        keys = Player.Get().Actions;
        currKey = 0;
        QTETicking = false;
        isBusy = false;
        timeQTE = maxQTETime;
        playerScene.isLocked = false;
    }

    internal override void Tick()
    {
        textPercent.text = $"{Mathf.RoundToInt((1 - (timeLeft / timeMax)) * 100)} %";

        if (QTETicking) timeQTE -= Time.deltaTime;
        if (timeQTE <= 0) GameOver();

        if (isBusy)
        {
            if (Input.anyKeyDown) TickQTE();
            return;
        }

        timeSpawn -= Time.deltaTime;
        if(timeSpawn <= 0)
        {
            timeSpawn = spawnRate;
            GetEnemyInstance();
        }

        foreach(CutoutBehaviour cutout in cutouts)
        {

            if (cutout == null) continue;

            if(Vector2.Distance(cutout.transform.position, playerScene.transform.position) < attackRange)
            {
                target = cutout;
                EngageQTE();
            }

            else if(Vector2.Distance(cutout.transform.position, new Vector3(objective, cutout.transform.position.y, 0)) < attackRange/2)
            {
                GameOver();
            }

        }

        

    }

    internal override void Over()
    {
        ZoomOut();
        KillAll();
    }

    internal override void Win()
    {
        ZoomOut();
        KillAll();
        carScene.SetActive(false);
    }

    void KillAll()
    {
        for(int i = 0; i < cutouts.Count; i++)
        {
            if (cutouts[i] == null) continue;
            Destroy(cutouts[i].gameObject);
        }
    }

    void TickQTE()
    {

        if (Input.GetKeyDown(qte[currKey]))
        {
            //next QTE + verify
            letterControllers[currKey].SetVisible(false, true);
            currKey++;

            if (currKey >= 3) LeaveQTE();

        }
        else GameOver();

    }

    void EngageQTE()
    {
        isBusy = true;
        //do...
        foreach(CutoutBehaviour cutout in cutouts)
        {
            if (cutout == null) continue;
            cutout.enabled = false;
        }

        //Assign the new QTE + Reset counter
        qte = new KeyCode[3];
        for(int i = 0; i < 3; i++) {
            qte[i] = keys[Random.Range(0, keys.Length)];
            letterTexts[i].text = qte[i].ToString();
        }
        currKey = 0;


        // Lock Player
        playerScene.isLocked = true;

        // Camera Focus
        Camera.main.transform.LeanMoveX(playerScene.transform.position.x,timeCameraQte).setEase(easeCameraQte);
        Camera.main.transform.LeanMoveY(-2, timeCameraQte).setEase(easeCameraQte);
        LeanTween.value(0, 1, timeCameraQte).setOnUpdate((float v) =>
        {
            Camera.main.orthographicSize = Mathf.Lerp(maxFocus, minFocus, v);

        }).setEase(easeCameraQte).setOnComplete(() =>
        {
            // Letters All-ShowUp
            lettersContainer.transform.position = new(
                target.transform.position.x, lettersContainer.transform.position.y, lettersContainer.transform.position.z
            );

            Camera.main.orthographicSize = minFocus;
            timeQTE = maxQTETime;
            QTETicking = true;

            foreach(ObjectsAminator control in letterControllers)
            {
                control.SetVisible(true, true);
            }

        });

    }

    void LeaveQTE()
    {
        //Camera Un-Focus
        ZoomOut();
        isBusy = false;
        timeQTE = maxQTETime;
        QTETicking = false;

        // Destroy the target
        Destroy(target.gameObject);

        // Unlock the player scene
        playerScene.isLocked = false;

        // Re-Enable the cutouts
        foreach (CutoutBehaviour cutout in cutouts)
        {
            if (cutout == null) continue;
            cutout.enabled = true;
        }

        for(int i = 0; i < cutouts.Count; i++) { if (cutouts[i] == null) cutouts.RemoveAt(i); }

    }

    void ZoomOut()
    {
        Camera.main.transform.LeanMoveX(0, timeCameraQte).setEase(easeCameraQte);
        Camera.main.transform.LeanMoveY(0, timeCameraQte).setEase(easeCameraQte);
        LeanTween.value(1, 0, timeCameraQte).setOnUpdate((float v) =>
        {
            Camera.main.orthographicSize = Mathf.Lerp(maxFocus, minFocus, v);

        }).setEase(easeCameraQte).setOnComplete(() =>
        {
            // Letters All-ShowUp
            Camera.main.orthographicSize = maxFocus;

            foreach (ObjectsAminator control in letterControllers)
            {
                control.SetVisible(false, true);
            }

        });
    }

    GameObject GetEnemyInstance()
    {
        int enemyID = Random.Range(0, 100) > 60 ? 0 : 1;
        Evil currEvil = GameManager.Get().currentEvil;
        
        GameObject enemy = Instantiate(enemyPrefabs[enemyID], points[Random.Range(0, 100) > 60 ? 0 : 1].position, Quaternion.identity);
        GameObject clothEnemy = Instantiate(currEvil.cloth.prefab, enemy.transform);
        clothEnemy.transform.localPosition = currEvil.cloth.offsets[enemyID];
        if(enemy.transform.Find("Tintable")) enemy.transform.Find("Tintable").GetComponent<SpriteRenderer>().color = currEvil.tint;

        cutouts.Add(enemy.GetComponent<CutoutBehaviour>());

        return enemy;
    }

}
