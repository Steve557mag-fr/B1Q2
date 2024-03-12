using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutBehaviour : MonoBehaviour
{
    [Header("Generals")]
    [SerializeField] bool once = false;

    [Header("Movements")]
    public float destination;
    [SerializeField] float horizontalWalkDistance = 1;
    [SerializeField] AnimationCurve verticalWalkCurve;
    [SerializeField] float groundHeight = 0;

    [Header("Animations")]
    [SerializeField] AnimationCurve derivateCurve;
    [SerializeField] float derivateFactor = 1f;
    [SerializeField] float derivateSpeed = 1;
    [SerializeField] float walkTimeAfterAnimation = 1f;
    [SerializeField] float walkTimeAnimation = 1f;
    [SerializeField] float facingTimerAfterAnimation = 1f;
    [SerializeField] float facingTimeAnimation = 1f;
    [SerializeField] LeanTweenType walkEase;
    [SerializeField] LeanTweenType facingEase;

    float timer;
    float prevS;

    public void Update()
    {
        // Handle Timer
        timer = Mathf.Max(0, timer-Time.deltaTime);
        if (timer > 0) return;

        // Calculate and Facing forward
        float sDestination = Mathf.Sign(destination - transform.position.x);
        if (sDestination != prevS)
        {
            // Rotate with LeanTween
            LeanTween.rotateY(gameObject, Mathf.Max(0, sDestination) * 180, facingTimeAnimation).setEase(facingEase);
            timer = facingTimeAnimation + facingTimerAfterAnimation;
            prevS = sDestination;
        }
        else
        {
            // Calculate End Position of the travel
            // Move with LeanTween
            float finalX = transform.position.x + horizontalWalkDistance * sDestination;
            LeanTween.moveX(gameObject, finalX, walkTimeAnimation).setEase(walkEase).setOnUpdate( (float v) => {
                transform.position = new Vector3(transform.position.x, groundHeight + verticalWalkCurve.Evaluate(v), 0);
                transform.localEulerAngles = new(0, transform.localEulerAngles.y, derivateCurve.Evaluate(v * derivateSpeed) * derivateFactor);
            }).setOnComplete(() => { if(once) { Destroy(gameObject); } });
            timer = walkTimeAnimation + walkTimeAfterAnimation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new(destination, groundHeight), 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-10, groundHeight, 0), new Vector3(10, groundHeight, 0));
        transform.position = new Vector3(transform.position.x, groundHeight, 0);
    }

    public void OnDestroy()
    {

        Instantiate(Resources.Load<GameObject>("Prefabs/SmokeParticle"), transform.position+Vector3.up * 1, Quaternion.identity);

    }

}
