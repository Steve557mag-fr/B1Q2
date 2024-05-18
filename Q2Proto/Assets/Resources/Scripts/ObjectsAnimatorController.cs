using System;
using UnityEngine;
using UnityEngine.Events;

public class ObjectsAnimatorController : MonoBehaviour
{
    [SerializeField] ObjectsAminator[] animators = new ObjectsAminator[0];
    [SerializeField] UnityEvent events;
    Action customAction = null;
    bool canAnimate = false;
    int ind = 0;

    private void Awake()
    {
        canAnimate = true;
    }
    public void Play(bool isVisible, Action callback = null, bool forcePlay = true)
    {
        print("boub from : " + gameObject.name);
        if ( animators == null ||animators.Length == 0) return;
        if (!canAnimate && !forcePlay) return;
        customAction = callback;
        canAnimate = false;
        ind = 0;

        foreach (ObjectsAminator animator in animators)
        {
            animator.SetVisible(isVisible, forcePlay, onFinish);
        }
    }
    public void ForceResetAll()
    {
        foreach(ObjectsAminator animator in animators)
        {
            animator.ForceReset();
        }
    }

    void onFinish()
    {
        ind++;
        if(ind >= animators.Length)
        {
            print(gameObject.name + " is done!");
            canAnimate = true;
            if(customAction != null)
            {
                print(customAction);
                customAction.Invoke();
                customAction = null;
            }
            events.Invoke();
        }
    }

}
