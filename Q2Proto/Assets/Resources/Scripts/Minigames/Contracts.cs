using System.Collections;
using UnityEngine;

public class Contracts : Minigame
{
    public RigidMovement playerScene;
    public GameObject[] cutoutPrefabs;
    public Transform[] points;
    public float chance = 65f;
    bool lockPosition;
    int currentIndex = 0;
    Enemy[] enemies;

    internal override void Begin()
    {
        wincase = false;
        currentIndex = 0;
        lockPosition = false;
        enemies = new Enemy[cutoutPrefabs.Length];

        for(int i = 0; i < cutoutPrefabs.Length; i++)
        {
            enemies[i] = new Enemy()
            {
                alreadyApproved = false,
                isEvil = Random.Range(0, 100) <= chance,
                cutout = Instantiate(cutoutPrefabs[i], points[i].transform.position, Quaternion.identity)
            };

            if (enemies[i].isEvil)
            {
                Evil currEvil = GameManager.Get().currentEvil;
                GameObject go = enemies[i].cutout;
                if (go.transform.Find("Tintable")) go.transform.Find("Tintable").GetComponent<SpriteRenderer>().color = currEvil.tint;
            }

        }

        playerScene.isLocked = true;
        playerScene.SetPosition(points[currentIndex].transform.position.x,null);

    }

    internal override void Tick()
    {

        int countInteractabled = 0;
        for(int i = 0; i < enemies.Length; i++)
        {
            countInteractabled += enemies[i].alreadyApproved ? 1 : 0;
        }
        if(countInteractabled >= enemies.Length)
        {
            GameWin();
            return;
        }

        if (lockPosition) return;
        print("Hey! ticked");

        if (Input.GetKeyDown(Player.Get().Left)) ToLeft();
        if (Input.GetKeyDown(Player.Get().Right)) ToRight();
        
        if (Input.GetKeyDown(Player.Get().Action1)) Approve();
        if (Input.GetKeyDown(Player.Get().Action2)) Denonce();

    }

    internal override void Over() {  KillAll(); }
    
    void ToLeft()
    {
        if (lockPosition) return;
        lockPosition = true;
        currentIndex = Mathf.Clamp(currentIndex-1, 0, points.Length-1);
        playerScene.SetPosition(points[currentIndex].transform.position.x, () => { lockPosition = false; });
    }

    void ToRight()
    {
        if (lockPosition) return;
        currentIndex = Mathf.Clamp(currentIndex+1, 0, points.Length-1);
        lockPosition = true;
        playerScene.SetPosition(points[currentIndex].transform.position.x, () => { lockPosition = false; });
    }

    void Approve()
    {
        if (enemies[currentIndex].alreadyApproved) return;

        if (enemies[currentIndex].isEvil)
        {
            GameOver();
        }
        else
        {
            enemies[currentIndex].alreadyApproved = true;
            enemies[currentIndex].cutout.GetComponent<CutoutBehaviour>().enabled = false;
        }

    }

    void Denonce() {
        if (enemies[currentIndex].alreadyApproved) return;

        if (!enemies[currentIndex].isEvil)
        {
            GameOver();
        }
        else
        {
            enemies[currentIndex].alreadyApproved = true;
            enemies[currentIndex].cutout.GetComponent<CutoutBehaviour>().enabled = false;
            enemies[currentIndex].cutout.transform.LeanMoveLocalY(-10, 1);
        }
    }


    void KillAll()
    {
        for(int i= 0;i < enemies.Length; i++)
        {
            Destroy(enemies[i].cutout);
        }
    }

}

[System.Serializable]
public struct Enemy
{
    public GameObject cutout;
    public bool alreadyApproved;
    public bool isEvil;
}
