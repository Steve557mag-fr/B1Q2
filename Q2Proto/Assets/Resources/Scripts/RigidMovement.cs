using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{

    public Player player;
    public float speed;
    public float minScene, maxScene;

    void Update()
    {

        transform.position += Vector3.right * Player.GetAxis(KeyCode.D, KeyCode.A) * speed * Time.deltaTime;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minScene, maxScene), transform.position.y);

    }

}
