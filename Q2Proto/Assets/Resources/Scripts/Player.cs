using UnityEngine;

public class Player : MonoBehaviour
{
    
    public bool isFreeze = false;
    public KeyCode left, right;
    [SerializeField] float speed = 2f;
    [SerializeField] float facingTimeAnimation;
    [SerializeField] LeanTweenType facingEase;

    [SerializeField] float minScene, maxScene;

    float oldPosition, prevS;

    int GetAxis(KeyCode A, KeyCode B)
    {
        return System.Convert.ToInt16(Input.GetKey(A)) - System.Convert.ToInt16(Input.GetKey(B));
    }

    void Update()
    {
        if (isFreeze) return;
        var axis = GetAxis(right, left);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + axis * speed * Time.deltaTime, minScene, maxScene), transform.position.y);

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

    public void SetDestination (float destination)
    {
        float distance = Mathf.Abs(transform.position.x - destination);
        if (distance < 0.01f) return;

        LeanTween.moveX(gameObject, destination, distance / speed);
    }

}
