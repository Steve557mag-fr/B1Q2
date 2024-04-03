using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class QTE : MiniGame
{
    [Header("Links Reference")]
    [SerializeField] Player player;
    [SerializeField] GameObject lettersContainer;
    [SerializeField] TextMeshPro[] letters;
    [SerializeField] GameObject needle;

    [Header("Camera Settings")]
    [SerializeField] Camera qteCamera;
    [SerializeField] float camerScaleInQTE, camerScaleOutQTE;

    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefabs;
    [SerializeField] List<Enemy> enemies;
    [SerializeField] Transform[] positions;

    [Header("QTE System")]
    [SerializeField] KeyCode[] keys;
    [SerializeField] float timePerQTE = 1;
    [SerializeField] float minRadius = 1.5f;
    [SerializeField] float nextEnemyTime;

    [Header("Generals")]
    [SerializeField] float maxTimer = 30;
    public UnityEvent eventsOnStart, eventsOnFinish;

    int currKey;
    KeyCode[] combo;
    Enemy currEnemy;
    float timerNextEnemy;
    bool qteBusy, lockKey;
    float globalTimer = 0;
    float currTime = 0;

    public void ExitQTEView()
    {
        // Animate the exit
        LeanTween.rotateX(lettersContainer, 90, 0.25f);
        LeanTween.move(qteCamera.gameObject, new Vector3(0, 0, -10f), 0.25f)
        .setOnUpdate((float e) =>
        {
            qteCamera.orthographicSize = Mathf.Lerp(camerScaleInQTE, camerScaleOutQTE, e);
        });
    }

    public void EnterQTEView(System.Action after)
    {
        //Reset Letter Signs
        foreach(TextMeshPro letter in letters)
        {
            letter.transform.parent.LeanMoveLocalY(2.5f, 0);
        }

        // Animate the entering
        LeanTween.move(lettersContainer, currEnemy.obj.transform.position, 0.25f).setOnComplete(() =>
        {
            LeanTween.rotateX(lettersContainer, 0, 0.25f);
            after();
        });

        // Center the camera on the action
        Vector3 target = new(currEnemy.obj.transform.position.x, currEnemy.obj.transform.position.y + 2.5f, -10f);
        LeanTween.move(qteCamera.gameObject, target, 0.25f)
        .setOnUpdate((float e) =>
        {
            qteCamera.orthographicSize = Mathf.Lerp(camerScaleOutQTE, camerScaleInQTE, e);
        });

    }

    public void StartQTE(Enemy e)
    {
        if (e.obj == null) return;
        
        // Lock during the starting phase ...
        qteBusy = true;
        currTime = 99;
        currEnemy = e;
        currEnemy.needCheck = false;
        
        // Freeze Enemies
        foreach (Enemy enemy in enemies)
        {
            enemy.obj.GetComponent<CutoutBehaviour>().enabled = false;
        }

        // Assign new combo
        combo = new KeyCode[letters.Length];
        for (var i = 0; i < letters.Length; i++)
        {
            combo[i] = keys[Random.Range(0, keys.Length)];
            letters[i].text = combo[i].ToString();
        }

        // Play the transition + unlock QTE
        EnterQTEView(() =>
        {
            currKey = 0;
            currTime = timePerQTE;
        });


    }

    public void FailQUTE()
    {
        lockKey = true;
        LooseGame(looseSequence);
    }
    
    public void SuccessQTE()
    {
        print($"[QTE]: SUCCESS at {currKey+1}/3");

        // Move the letter Sign
        letters[currKey].transform.parent.LeanMoveLocalY(0, 1);

        // Check if the QTE is finished
        if(currKey >= letters.Length - 1)
        {
            // Exit the QTE State
            ExitQTEView();
            qteBusy = false;

            // Unfreeze the monsters
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].obj.GetComponent<CutoutBehaviour>().enabled = true;
                if (enemies[i].obj == currEnemy.obj) enemies.RemoveAt(i);
            }

            // Destroy the Monster
            Destroy(currEnemy.obj);

        }
        else currKey++;
        
    }

    Vector3 PickPosition()
    {
        return Random.Range(1, 100) >= 50 ? positions[0].position : positions[1].position;
    }
    
    public override void StartGame(GameObject area)
    {
        base.StartGame(area, () =>
        {
            eventsOnStart.Invoke();
        });

        player.GetComponent<SpriteRenderer>().enabled = true;
        player.isFreeze = false;
        enemies = new();
        timerNextEnemy = Random.Range(1, nextEnemyTime);
        globalTimer = maxTimer;
        lockKey = false;
        qteBusy = false; 
    }

    public override void UpdateGame()
    {
        base.UpdateGame();

        // Check the global timer
        float needleOffset = (1 - (globalTimer / 30));
        needle.transform.localPosition = transform.right * needleOffset - 0.5f * transform.right - 0.1f * transform.up;

        if (globalTimer <= 0) WinGame();

        if (!qteBusy)
        {
            // detect qte activation
            foreach (Enemy enemy in enemies)
            {
                if (enemy.obj == null) continue;
                if (Vector3.Distance(enemy.obj.transform.position, player.transform.position) <= minRadius)
                {
                    StartQTE(enemy);
                    return;
                }
            }
            
            // enemies spawner
            if(timerNextEnemy <= 0)
            {
                timerNextEnemy = nextEnemyTime;
                // INSERT RANDOM EVIL :D
                GameObject enemySpawned = GameManager.GetEnemy(true, 2);
                enemySpawned.transform.position = PickPosition();
                enemySpawned.transform.parent = gArea.transform;
                enemies.Add(new() { obj = enemySpawned , needCheck = true});
            }
            timerNextEnemy -= Time.deltaTime;
            globalTimer -= Time.deltaTime;
            return;

        }

        // Qte Handler :
        // Timer for auto-failure
        if (currTime <= 0) {
            currTime = 999;
            FailQUTE();
            return;
        }
        currTime -= Time.deltaTime;

        if (!Input.anyKeyDown || Input.GetKey(player.left) || Input.GetKey(player.right) || lockKey) return;
        if (Input.GetKeyDown(combo[currKey])) SuccessQTE();
        else FailQUTE();

    }

    public override void WinGame()
    {
        eventsOnFinish.Invoke();
        base.WinGame();
    }

    public override void LooseGame(PlayableDirector obj)
    {
        ExitQTEView();
        eventsOnFinish.Invoke();
        base.LooseGame(obj);
    }

}

public struct Enemy
{

    public GameObject obj;
    public bool needCheck;

}
