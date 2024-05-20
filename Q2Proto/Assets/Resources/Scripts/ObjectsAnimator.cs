using System;
using UnityEditor;
using UnityEngine;

public class ObjectsAminator : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] Vector3 visiblePosition;
    [SerializeField] Vector3 invisiblePosition;
    [SerializeField] bool isLocalPosition = false;
    [SerializeField] AnimationCurve rotationCurve = new AnimationCurve(new Keyframe[] { new(0,0), new(1,0) });
    [SerializeField] float rotationAmplifier = 1f;

    bool canTween = false;

    [Header("Tween Settings")]
    [SerializeField] float delayTime = 0.0f;
    [SerializeField] float tweenTime = 0.1f;
    [SerializeField] LeanTweenType tweenType = LeanTweenType.easeInOutQuint;


    private void Awake()
    {
        ForceReset();
    }

    public void SetVisible(bool isVisible, bool forceTween = false,Action callback = null)
    {
        if (!canTween && !forceTween) return;
        canTween = false;
        Vector3 stPosition = transform.position;
        LeanTween.value(0, 1, tweenTime).setDelay(delayTime).setEase(tweenType).setOnUpdate((float t) =>
        {
            transform.position = Vector3.Lerp(stPosition, isVisible ? VisiblePosition : InvisiblePosition, t);
            transform.localEulerAngles = Vector3.forward * rotationAmplifier * rotationCurve.Evaluate(t);
        }).setOnComplete(() =>
        {
            
            if (callback != null) callback.Invoke();
        });
    }
    public void ForceReset()
    {
        transform.position = InvisiblePosition;
        canTween = true;
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = canTween ? Color.white : Color.red;
        Gizmos.DrawSphere(InvisiblePosition, 0.1f);
        Gizmos.DrawSphere(VisiblePosition,   0.1f);

        Gizmos.color = canTween ? Color.gray : Color.green;
        Gizmos.DrawLine(InvisiblePosition, VisiblePosition);

    }

    public Vector3 VisiblePosition
    {
        get { return (isLocalPosition ? transform.parent.position : Vector3.zero) + visiblePosition; }
    }
    public Vector3 InvisiblePosition
    {
        get { return (isLocalPosition ? transform.parent.position : Vector3.zero) + invisiblePosition; }
    }

}
