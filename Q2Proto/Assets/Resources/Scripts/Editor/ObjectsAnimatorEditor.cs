using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectsAminator))]
public class ObjectsAnimatorEditor : Editor
{

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        if(GUILayout.Button("Test Tween [RUNTIME ONLY]"))
        {
            ObjectsAminator myAnimator = (ObjectsAminator)target;
            myAnimator.SetVisible(true, false, () =>
            { 
                myAnimator.SetVisible(false);
            });
        }

    }
}
