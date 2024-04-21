using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Fighter : MiniGame
{
    [Header("Game Settings")]
    public List<Enemy> enemies = new List<Enemy>();
    public Transform[] points;
    public float maxEnemyTime = 3;
    public float maxQTETime = 4;
    public float qteDistance;
    public TextMeshPro gaugeFuel;

    public float cameraOutSize, cameraInSize;
    public float cameraTransitionTime;
    public LeanTweenType cameraTransitionEase;

    public Transform containerLetters;
    public Transform[] stickLetters;
    public TextMeshPro[] letters;

    public float enemyObjectif = 1.30f;

    float timeForEnemy, timeForQTE;
    bool isBusy;

    Enemy currentEnemy;
    int currentComboIndex = 0;
    int[] keyCombo;

    #region Overrides
    internal override void Setup()
    {
        enemies = new List<Enemy>();
        Player.instance.SetVisible(true);
        Player.instance.ResetPosition();
    }
    internal override void Begin()
    {
        SetTimer(30);
        Player.instance.isFreeze = false;
    }
    internal override void Tick()
    {
        if (isBusy)
        {

            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(Player.instance.keys[keyCombo[currentComboIndex]])) SuccessQTE();
                else GameOver();
            }

            if (timeForQTE > 0) timeForQTE = Mathf.Clamp(timeForQTE - Time.deltaTime, 0, maxQTETime);
            else GameOver();
            return;
        }

        gaugeFuel.text = $"{Mathf.Floor((1 - (timeLeft / 30f)) * 100)} %";

        if (timeForEnemy > 0) timeForEnemy = Mathf.Clamp(timeForEnemy - Time.deltaTime, 0, maxEnemyTime);
        else SpawnEnemy();

        for(int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy.cutout == null)
            {
                enemies.RemoveAt(i);
                continue;
            }
            if(Vector2.Distance(Player.instance.transform.position, enemy.cutout.transform.position) <= qteDistance && enemy.canDetected)
            {
                currentEnemy = enemy;
                enemy.canDetected = false;
                EnterQTE(enemy.cutout.transform.position.x * Vector3.right + Vector3.up * -2.0f + Vector3.forward * -10);
            }

            if (Mathf.Abs (enemyObjectif - enemy.cutout.transform.position.x) < .5f)
            {
                GameOver();
            }

        }
    }
    internal override void Over()
    {
        Player.instance.isFreeze = true;
        foreach(var enemy in enemies)
        {
            Destroy(enemy.cutout.gameObject);
        }
        enemies = new List<Enemy>();

        ExitQTE();
    }
    internal override void Win()
    {
        Player.instance.isFreeze = true;
        foreach (var enemy in enemies)
        {
            Destroy(enemy.cutout.gameObject);
        }
        enemies = new List<Enemy>();
    }
    internal override void TimerEnded()
    {
        GameWin();
    }

    #endregion
    #region Utils
    void EnterQTE(Vector3 cameraPosition)
    {
        if (isBusy) return;
        isBusy = true;
        SetActiveTimer(false);

        foreach(Enemy enemeny in enemies)
        {
            enemeny.cutout.enabled = false;
        }

        Camera.main.transform.LeanMove(cameraPosition,cameraTransitionTime).setEase(cameraTransitionEase);
        LeanTween.value(cameraOutSize, cameraInSize, cameraTransitionTime).setEase(cameraTransitionEase).setOnUpdate((v) =>
        {
            Camera.main.orthographicSize = v;
        });

        keyCombo = new int[]
        {
            Random.Range(0,2),
            Random.Range(0,2),
            Random.Range(0,2)
        };
        currentComboIndex = 0;
        timeForQTE = maxQTETime;

        containerLetters.transform.localPosition = new(cameraPosition.x, -5f, 0f);
        for(int i = 0; i < 3; i++)
        {
            letters[i].text = Player.instance.keys[keyCombo[i]].ToString();
            stickLetters[i].transform.localEulerAngles = Vector3.zero;
        }
    }

    void SuccessQTE()
    {
        currentComboIndex++;

        for (int i = 0; i < currentComboIndex; i++)
        {
            stickLetters[i].transform.LeanRotateX(90, 0.1f);
        }

        if (currentComboIndex >= keyCombo.Length)
        {
            Destroy(currentEnemy.cutout.gameObject);
            ExitQTE();
            
            return;
        }

    }

    void ExitQTE()
    {
        Camera.main.transform.LeanMove(new Vector3(0,0,-10), cameraTransitionTime).setEase(cameraTransitionEase);
        LeanTween.value(cameraInSize, cameraOutSize, cameraTransitionTime).setEase(cameraTransitionEase).setOnUpdate((v) =>
        {
            Camera.main.orthographicSize = v;
        });

        foreach (Enemy enemeny in enemies)
        {
            enemeny.cutout.enabled = true;
        }

        containerLetters.transform.localPosition = new(12f, -5f, 0);
        isBusy = false;
        SetActiveTimer(true);
    }

    void SpawnEnemy() // [!] OUTFIT SYSTEM NEEDED
    {
        timeForEnemy = maxEnemyTime;
        var enemyID = Random.Range(0, 1);
        var clone = Instantiate(Resources.Load<GameObject>($"Prefabs/Enemies/Enemy_{enemyID}"),Gameplay.layerTR);
        clone.transform.position = points[Random.Range(0, points.Length)].transform.position;

        EvilProfile p = GameManager.instance.evilProfile;
        var outfit = Instantiate(p.outfitID.asset, clone.transform);
        outfit.transform.localPosition = p.outfitID.positions[enemyID].position;

        var a = clone.transform.Find("Tintable");
        if (a != null)
        {
            a.GetComponent<SpriteRenderer>().color = p.color;
        }

        var enemy = new Enemy();
        enemy.cutout = clone.GetComponent<CutoutBehaviour>();
        enemy.canDetected = true;

        enemies.Add(enemy);

    }
    #endregion
}

[System.Serializable]
public struct Enemy
{
    public bool canDetected;
    public CutoutBehaviour cutout;
}
