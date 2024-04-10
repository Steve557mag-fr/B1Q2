using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MiniGame
{
    [Header("Game Settings")]
    public List<Enemy> enemies = new List<Enemy>();
    public Transform[] points;
    public float maxEnemyTime = 3;
    public float maxQTETime = 4;
    public Transform gaugeFuel;

    float timeForEnemy, timeForQTE;
    bool isBusy;

    int currentComboIndex = 0;
    int[] keyCombo;

    #region Overrides
    internal override void Setup()
    {
        enemies = new List<Enemy>();
        Player.instance.SetVisible(true);
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

            if (Input.anyKey)
            {
                if (Input.GetKeyDown(Player.instance.keys[keyCombo[currentComboIndex]])) SuccessQTE();
                else GameOver();
            }

            if (timeForQTE > 0) timeForQTE = Mathf.Clamp(timeForQTE - Time.deltaTime, 0, maxQTETime);
            else GameOver();
            return;
        }

        if (timeForEnemy > 0) timeForEnemy = Mathf.Clamp(timeForEnemy - Time.deltaTime, 0, maxEnemyTime);
        else SpawnEnemy();

        foreach(var enemy in enemies)
        {
            if(Vector2.Distance(Player.instance.transform.position, enemy.cutout.transform.position) < 2 && enemy.canDetected)
            {
                EnterQTE();
            }
        }
    }
    internal override void Over()
    {
        Player.instance.isFreeze = true;
    }
    internal override void Win()
    {
        Player.instance.isFreeze = true;
    }
    internal override void TimerEnded()
    {
        GameWin();
    }

    #endregion
    #region Utils
    void EnterQTE()
    {

    }

    void SuccessQTE()
    {

    }

    void ExitQTE()
    {

    }

    void SpawnEnemy()
    {
        timeForEnemy = maxEnemyTime;
        var clone = Instantiate(Resources.Load<GameObject>($"Prefabs/Enemies/Enemy_{Random.Range(0, 1)}"),Gameplay.layerTR);
        clone.transform.position = points[Random.Range(0, points.Length)].transform.position;

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
