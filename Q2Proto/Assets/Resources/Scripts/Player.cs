using UnityEngine;

public class Player : MonoBehaviour
{
    public KeyCode[] keys;
    public bool isFreeze;
    public float speed;

    public static int GetAxisDown(KeyCode A, KeyCode B) { return System.Convert.ToInt16(Input.GetKeyDown(A)) - System.Convert.ToInt16(Input.GetKeyDown(B)); }
    public static int GetAxis(KeyCode A, KeyCode B) { return System.Convert.ToInt16(Input.GetKey(A)) - System.Convert.ToInt16(Input.GetKey(B)); }
    public void SetVisible(bool isVisible) { GetComponent<SpriteRenderer>().enabled = isVisible; }

    private void Update()
    {
        if (isFreeze) return;
        transform.position += Vector3.right * GetAxis(KeyCode.D, KeyCode.A) * speed * Time.deltaTime;
    }

    public static Player instance
    {
        get { return FindAnyObjectByType<Player>(); }
    }

}
