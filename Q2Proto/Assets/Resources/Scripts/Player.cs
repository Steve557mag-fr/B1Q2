using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 2f;
    public float facingTimeAnimation;
    public KeyCode left, right;
    public LeanTweenType facingEase;
    float oldPosition, prevS;
    
    int GetAxis(KeyCode A, KeyCode B)
    {
        return System.Convert.ToInt16(Input.GetKey(A)) - System.Convert.ToInt16(Input.GetKey(B));
    }

    void Update()
    {
        var axis = GetAxis(right, left);
        transform.position += Vector3.right * axis * speed * Time.deltaTime;

        // Calculate and Facing forward
        float sDestination = Mathf.Sign(oldPosition - transform.position.x);
        if (axis == 0) return;
        if (sDestination != prevS)
        {
            // Rotate with LeanTween
            LeanTween.rotateY(gameObject, Mathf.Max(0, sDestination) * 180, facingTimeAnimation).setEase(facingEase);
            prevS = sDestination;
        }
        oldPosition = transform.position.x;

    }
}
