using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class QTE : MiniGame
{
    [Header("Links Reference")]
    public Player player;
    public GameObject qteSignGO;
    public TextMeshPro[] keysGO;

    [Header("Enemies")]
    public GameObject enemyPrefabs;
    [SerializeField] List<Enemy> enemies;
    public Transform[] positions;

    [Header("QTE System")]
    public KeyCode[] keys;
    public float timePerQTE = 1;
    public float minRadius = 1.5f;
    public float nextEnemyTime;

    float globalTimer = 0;

    Enemy currEnemy;
    float currNextEnemy;
    KeyCode[] keysQTE;
    int currKey;
    bool qteBusy;

    [HideInInspector] public float currTime;

    public void StartQTE(Enemy e)
    {
        if (e.obj == null) return;
        //Pick random keys
        keysQTE = new KeyCode[keysGO.Length];
        currTime = 99;
        qteBusy = true;
        currEnemy = e;
        currEnemy.needCheck = false;
        print("[QTE]: WAITING PLAYER REACT | " + currKey.ToString());

        foreach (Enemy enemy in enemies)
        {
            enemy.obj.GetComponent<CutoutBehaviour>().enabled = false;
        }

        LeanTween.move(qteSignGO, e.obj.transform.position, 0.25f).setOnComplete(() =>
        {
            LeanTween.rotateX(qteSignGO, 0, 0.25f);
        
            for(var i = 0; i < keysGO.Length; i++)
            {
                keysQTE[i] = keys[Random.Range(0, keys.Length)];
                keysGO[i].text = keysQTE[i].ToString();
                keysGO[i].transform.parent.LeanMoveLocalY(2.5f, 0);
            }
            currKey = 0;
            currTime = timePerQTE;
        });
    }

    public void FailQUTE()
    {
        print("[QTE]: FAIL");
        LeanTween.rotateX(qteSignGO, 90, 0.25f);
        RestartGame();
    }

    public void SuccessQTE()
    {
        keysGO[currKey].transform.parent.LeanMoveLocalY(0, 1);
        print($"[QTE]: SUCCESS at {currKey+1}/3");

        if(currKey >= keysGO.Length - 1)
        {
            LeanTween.rotateX(qteSignGO, 90, 0.25f);
            qteBusy = false;


            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                print(enemy.obj);
                if (enemy.obj == currEnemy.obj) {
                    enemies.RemoveAt(i);
                    continue;
                }
            }
            Destroy(currEnemy.obj);

            enemies.ForEach((e) => { e.obj.GetComponent<CutoutBehaviour>().enabled = true; });

            return;
        }
        else currKey++;

        

    }

    public override void StartGame(GameObject area)
    {
        base.StartGame(area);
        print("[MG1]: Started!");
        enemies = new();
        currNextEnemy = Random.Range(1, nextEnemyTime);
        globalTimer = 30;
        qteBusy = false;
    }

    Vector3 PickPosition()
    {
        return Random.Range(1, 100) >= 50 ? positions[0].position : positions[1].position;
    }

    public override void UpdateGame()
    {

        if (globalTimer <= 0) WinGame();

        if (!qteBusy)
        {
            // detect qte activation
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.needCheck || enemy.obj == null) continue;
                if (Vector3.Distance(enemy.obj.transform.position, player.transform.position) <= minRadius)
                {
                    StartQTE(enemy);
                    return;
                }
            }
        
            // enemies spawner
            if(currNextEnemy <= 0)
            {
                currNextEnemy = nextEnemyTime;
                // INSERT RANDOM EVIL :D
                GameObject spawned = Instantiate(enemyPrefabs, PickPosition(), Quaternion.identity);
                spawned.GetComponent<CutoutBehaviour>().destination = 0;
                spawned.transform.parent = gArea.transform;
                enemies.Add(new() { obj = spawned , needCheck = true});
            }
            currNextEnemy -= Time.deltaTime;
            globalTimer -= Time.deltaTime;

            return;
        }

        // Qte Handler :
        // Timer for auto-failure
        if (currTime <= 0) {
            FailQUTE();
            return;
        }
        currTime -= Time.deltaTime;


        if (!Input.anyKeyDown || Input.GetKey(player.left) || Input.GetKey(player.right)) return;
        if (Input.GetKeyDown(keysQTE[currKey])) SuccessQTE();
        else FailQUTE();

    }
}

public struct Enemy
{

    public GameObject obj;
    public bool needCheck;

}
