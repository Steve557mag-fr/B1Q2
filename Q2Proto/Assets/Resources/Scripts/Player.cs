using UnityEngine;

public class Player : MonoBehaviour
{
    
    public bool isFreeze = false;
    public KeyCode left, right;
    [SerializeField] float speed = 2f;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] float minPosition, maxPosition;

    [SerializeField] LeanTweenType facingEase;
    [SerializeField] float facingTime;

    [SerializeField] LeanTweenType transitionEase;
    [SerializeField] float transitionTime;

    float oldPosition, prevS;

    public static int GetAxis(KeyCode A, KeyCode B)
    {
        return System.Convert.ToInt16(Input.GetKey(A)) - System.Convert.ToInt16(Input.GetKey(B));
    }

    void Update()
    {
        if (isFreeze) return;
        var axis = GetAxis(right, left);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + axis * speed * Time.deltaTime, minPosition, maxPosition), transform.position.y);

        // Calculate and Facing forward
        float sDestination = Mathf.Sign(oldPosition - transform.position.x);
        if (axis == 0) return;
        if (sDestination != prevS)
        {
            // Rotate with LeanTween
            LeanTween.rotateY(gameObject, Mathf.Max(0, sDestination) * 180, facingTime).setEase(facingEase);
            prevS = sDestination;
        }
        oldPosition = transform.position.x;

    }

    public void SetDestination (float destination, System.Action onComplete)
    {
        float distance = Mathf.Abs(transform.position.x - destination);
        if (distance < 0.01f) return;

        LeanTween.moveX(gameObject, destination, transitionTime).setEase(transitionEase).setOnComplete(onComplete);
    }
        
    public void SetVisible(bool isVisible)
    {
        playerSprite.enabled = isVisible;
    }

}
