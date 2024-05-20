using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderPlayer;
    [SerializeField] Sprite spriteRun, spriteIdle;
    [SerializeField] float speed;
    [SerializeField] float sceneMin, sceneMax;
    [SerializeField] bool isLocked;
    
    void Update()
    {

        if (isLocked) return;

        float input = Player.GetAxis(KeyCode.D, KeyCode.A);
        transform.position += Vector3.right * input * speed * Time.deltaTime;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, sceneMin, sceneMax), transform.position.y);

        if (input == 0) return;
        renderPlayer.flipX = System.Convert.ToBoolean(Mathf.Max(0, Mathf.Sign(input)));

    }

}
