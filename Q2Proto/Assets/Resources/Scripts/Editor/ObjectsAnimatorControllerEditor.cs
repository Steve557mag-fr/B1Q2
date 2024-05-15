using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectsAnimatorController))]
public class ObjectsAnimatorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if(GUILayout.Button("TEST [RUNTIME ONLY]"))
        {

            ObjectsAnimatorController objectsAnimator = (ObjectsAnimatorController)target;
            objectsAnimator.Play(true);

            LeanTween.delayedCall(5, () => {
                objectsAnimator.Play(false);
            });

        }

    }
}
