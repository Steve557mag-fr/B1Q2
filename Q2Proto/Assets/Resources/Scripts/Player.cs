using UnityEngine;

public class Player : MonoBehaviour
{
    public KeyCode[] keys;
    public bool isFreeze;
    public float speed;
    public Vector3 defaultPosition;
    public SpriteRenderer[] spriteSlots;

    public Sprite idle, run;

    bool canTween = true;
    bool canTweenOnRotate = true;

    public static int GetAxisDown(KeyCode A, KeyCode B) { return System.Convert.ToInt16(Input.GetKeyDown(A)) - System.Convert.ToInt16(Input.GetKeyDown(B)); }
    public static int GetAxis(KeyCode A, KeyCode B) { return System.Convert.ToInt16(Input.GetKey(A)) - System.Convert.ToInt16(Input.GetKey(B)); }
    public void SetVisible(bool isVisible) { foreach(var s in spriteSlots) { s.enabled = isVisible; } }

    private void Update()
    {
        if (isFreeze) return;
        var axis = GetAxis(KeyCode.D, KeyCode.A);
        transform.position += Vector3.right * GetAxis(KeyCode.D, KeyCode.A) * speed * Time.deltaTime;

        spriteSlots[0].sprite = (axis == 0) ? idle : run; 

        if (axis == 0 || !canTweenOnRotate) return;
        canTweenOnRotate = false;
        transform.LeanRotateY(180 * (axis > 0 ? 1 : 0), .1f).setOnComplete(() => { canTweenOnRotate = true; });

    }

    public void SetDestination(float x)
    {
        if (!canTween) return;
        canTween = false;
        transform.LeanMoveLocalX(x, 0.5f).setOnComplete(() => { canTween = true;});
    }

    public void ResetPosition()
    {
        transform.position = defaultPosition;
    }

    public bool CanMove
    {
        get { return canTween; }
    }

    public static Player instance
    {
        get { return FindAnyObjectByType<Player>(); }
    }

}
